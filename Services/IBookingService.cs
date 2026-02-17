using SportsBookingPlatform.DTOs;

namespace SportsBookingPlatform.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto, int userId);
    Task<BookingResponseDto> ConfirmBookingAsync(ConfirmBookingDto confirmBookingDto, int userId);
    Task<BookingResponseDto> CancelBookingAsync(CancelBookingDto cancelBookingDto, int userId);
    Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(int userId);
    Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId);
}
