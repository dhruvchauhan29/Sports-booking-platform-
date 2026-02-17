using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public class SlotService : ISlotService
{
    private readonly ISlotRepository _slotRepository;
    private readonly ICourtRepository _courtRepository;

    public SlotService(ISlotRepository slotRepository, ICourtRepository courtRepository)
    {
        _slotRepository = slotRepository;
        _courtRepository = courtRepository;
    }

    public async Task<IEnumerable<SlotResponseDto>> GetAvailableSlotsAsync(AvailableSlotQueryDto query)
    {
        var slots = await _slotRepository.GetAvailableSlotsAsync(
            query.VenueId,
            query.CourtId,
            query.SportType,
            query.StartDate,
            query.EndDate
        );

        return slots.Select(s => new SlotResponseDto
        {
            SlotId = s.SlotId,
            CourtId = s.CourtId,
            CourtName = s.Court.Name,
            VenueName = s.Court.Venue.Name,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Status = s.Status,
            CurrentPrice = s.CurrentPrice,
            ViewerCount = s.ViewerCount
        });
    }

    public async Task<LockSlotResponseDto> LockSlotAsync(int slotId, int userId)
    {
        var lockToken = Guid.NewGuid().ToString();
        var lockedUntil = DateTime.UtcNow.AddMinutes(5);

        var success = await _slotRepository.LockSlotAsync(slotId, lockToken, lockedUntil);
        
        if (!success)
        {
            throw new InvalidOperationException("Slot is not available for locking");
        }

        return new LockSlotResponseDto
        {
            SlotId = slotId,
            LockToken = lockToken,
            LockedUntil = lockedUntil
        };
    }

    public async Task<bool> UnlockSlotAsync(int slotId, string lockToken)
    {
        return await _slotRepository.UnlockSlotAsync(slotId, lockToken);
    }

    public async Task<SlotResponseDto> CreateSlotAsync(CreateSlotDto createSlotDto)
    {
        var court = await _courtRepository.GetByIdAsync(createSlotDto.CourtId);
        if (court == null)
        {
            throw new InvalidOperationException("Court not found");
        }

        var dynamicPrice = CalculateDynamicPrice(
            court.BasePrice,
            0, // Initial viewer count
            createSlotDto.StartTime,
            court.Rating
        );

        var slot = new Slot
        {
            CourtId = createSlotDto.CourtId,
            StartTime = createSlotDto.StartTime,
            EndTime = createSlotDto.EndTime,
            Status = BookingStatus.Available,
            CurrentPrice = dynamicPrice,
            ViewerCount = 0
        };

        var createdSlot = await _slotRepository.CreateAsync(slot);
        
        // Reload with navigation properties
        var slotWithRelations = await _slotRepository.GetByIdAsync(createdSlot.SlotId);
        
        return new SlotResponseDto
        {
            SlotId = slotWithRelations!.SlotId,
            CourtId = slotWithRelations.CourtId,
            CourtName = slotWithRelations.Court.Name,
            VenueName = slotWithRelations.Court.Venue.Name,
            StartTime = slotWithRelations.StartTime,
            EndTime = slotWithRelations.EndTime,
            Status = slotWithRelations.Status,
            CurrentPrice = slotWithRelations.CurrentPrice,
            ViewerCount = slotWithRelations.ViewerCount
        };
    }

    public decimal CalculateDynamicPrice(
        decimal basePrice, 
        int viewerCount, 
        DateTime slotStartTime, 
        double courtRating)
    {
        // Demand multiplier based on viewer count
        decimal demandMultiplier = viewerCount switch
        {
            0 or 1 => 1.0m,
            >= 2 and <= 5 => 1.2m,
            > 5 => 1.5m,
            _ => 1.0m
        };

        // Time-based multiplier (how soon the slot is)
        var hoursUntilSlot = (slotStartTime - DateTime.UtcNow).TotalHours;
        decimal timeMultiplier = hoursUntilSlot switch
        {
            > 24 => 1.0m,
            >= 6 and <= 24 => 1.2m,
            < 6 => 1.5m,
            _ => 1.0m
        };

        // Historical multiplier based on court rating
        decimal historicalMultiplier = courtRating switch
        {
            >= 1.0 and < 3.0 => 1.0m,
            >= 3.0 and < 4.0 => 1.2m,
            >= 4.0 and <= 5.0 => 1.5m,
            _ => 1.0m
        };

        // Calculate final price
        var finalPrice = basePrice * demandMultiplier * timeMultiplier * historicalMultiplier;
        
        return Math.Round(finalPrice, 2);
    }
}
