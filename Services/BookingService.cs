using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ApplicationDbContext _context;

    public BookingService(
        IBookingRepository bookingRepository,
        ISlotRepository slotRepository,
        IWalletRepository walletRepository,
        ApplicationDbContext context)
    {
        _bookingRepository = bookingRepository;
        _slotRepository = slotRepository;
        _walletRepository = walletRepository;
        _context = context;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto, int userId)
    {
        // Check for idempotency
        var existingBooking = await _bookingRepository.GetByIdempotencyKeyAsync(createBookingDto.IdempotencyKey);
        if (existingBooking != null)
        {
            return MapToDto(existingBooking);
        }

        var slot = await _slotRepository.GetByIdAsync(createBookingDto.SlotId);
        if (slot == null)
        {
            throw new InvalidOperationException("Slot not found");
        }

        if (slot.Status != BookingStatus.Locked)
        {
            throw new InvalidOperationException("Slot must be locked before creating a booking");
        }

        var booking = new Booking
        {
            UserId = userId,
            SlotId = createBookingDto.SlotId,
            Status = BookingStatus.Pending,
            FinalPrice = slot.CurrentPrice,
            CreatedAt = DateTime.UtcNow,
            IdempotencyKey = createBookingDto.IdempotencyKey
        };

        var createdBooking = await _bookingRepository.CreateAsync(booking);
        return MapToDto(createdBooking);
    }

    public async Task<BookingResponseDto> ConfirmBookingAsync(ConfirmBookingDto confirmBookingDto, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(confirmBookingDto.BookingId);
            if (booking == null || booking.UserId != userId)
            {
                throw new InvalidOperationException("Booking not found");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Booking is not in pending status");
            }

            // Verify lock token
            var slot = await _slotRepository.GetByIdAsync(booking.SlotId);
            if (slot == null || slot.LockToken != confirmBookingDto.LockToken)
            {
                throw new InvalidOperationException("Invalid lock token");
            }

            // Check wallet balance
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < booking.FinalPrice)
            {
                throw new InvalidOperationException("Insufficient wallet balance");
            }

            // Debit wallet
            wallet.Balance -= booking.FinalPrice;
            await _walletRepository.UpdateAsync(wallet);

            // Create transaction record
            var walletTransaction = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                Type = TransactionType.Debit,
                Amount = booking.FinalPrice,
                BalanceAfter = wallet.Balance,
                Description = $"Booking #{booking.BookingId} for slot #{booking.SlotId}",
                CreatedAt = DateTime.UtcNow,
                IdempotencyKey = $"booking-{booking.BookingId}-{Guid.NewGuid()}"
            };
            await _walletRepository.CreateTransactionAsync(walletTransaction);

            // Confirm booking
            booking.Status = BookingStatus.Confirmed;
            booking.ConfirmedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);

            // Update slot status
            slot.Status = BookingStatus.Confirmed;
            slot.LockToken = null;
            slot.LockedUntil = null;
            await _slotRepository.UpdateAsync(slot);

            await transaction.CommitAsync();

            return MapToDto(booking);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<BookingResponseDto> CancelBookingAsync(CancelBookingDto cancelBookingDto, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(cancelBookingDto.BookingId);
            if (booking == null || booking.UserId != userId)
            {
                throw new InvalidOperationException("Booking not found");
            }

            if (booking.Status != BookingStatus.Confirmed && booking.Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Booking cannot be cancelled");
            }

            var slot = await _slotRepository.GetByIdAsync(booking.SlotId);
            if (slot == null)
            {
                throw new InvalidOperationException("Slot not found");
            }

            // Calculate refund based on time until slot start
            decimal refundPercentage = CalculateRefundPercentage(slot.StartTime);
            decimal refundAmount = booking.FinalPrice * refundPercentage;

            if (refundAmount > 0 && booking.Status == BookingStatus.Confirmed)
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet != null)
                {
                    wallet.Balance += refundAmount;
                    await _walletRepository.UpdateAsync(wallet);

                    var walletTransaction = new WalletTransaction
                    {
                        WalletId = wallet.WalletId,
                        Type = TransactionType.Credit,
                        Amount = refundAmount,
                        BalanceAfter = wallet.Balance,
                        Description = $"Refund for cancelled booking #{booking.BookingId}",
                        CreatedAt = DateTime.UtcNow,
                        IdempotencyKey = $"refund-{booking.BookingId}-{Guid.NewGuid()}"
                    };
                    await _walletRepository.CreateTransactionAsync(walletTransaction);
                }
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = cancelBookingDto.CancellationReason;
            booking.RefundAmount = refundAmount;
            await _bookingRepository.UpdateAsync(booking);

            // Release slot
            slot.Status = BookingStatus.Available;
            slot.LockToken = null;
            slot.LockedUntil = null;
            await _slotRepository.UpdateAsync(slot);

            await transaction.CommitAsync();

            return MapToDto(booking);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);
        return bookings.Select(MapToDto);
    }

    public async Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        return booking == null ? null : MapToDto(booking);
    }

    private decimal CalculateRefundPercentage(DateTime slotStartTime)
    {
        var hoursUntilStart = (slotStartTime - DateTime.UtcNow).TotalHours;
        
        return hoursUntilStart switch
        {
            >= 24 => 1.0m,      // 100% refund
            >= 6 and < 24 => 0.5m, // 50% refund
            < 6 => 0.0m,        // No refund
            _ => 0.0m
        };
    }

    private BookingResponseDto MapToDto(Booking booking)
    {
        return new BookingResponseDto
        {
            BookingId = booking.BookingId,
            UserId = booking.UserId,
            SlotId = booking.SlotId,
            Status = booking.Status,
            FinalPrice = booking.FinalPrice,
            RefundAmount = booking.RefundAmount,
            CreatedAt = booking.CreatedAt,
            ConfirmedAt = booking.ConfirmedAt,
            CancelledAt = booking.CancelledAt,
            CancellationReason = booking.CancellationReason
        };
    }
}
