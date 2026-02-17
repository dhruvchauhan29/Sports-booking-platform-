using SportsBookingPlatform.DTOs;

namespace SportsBookingPlatform.Services;

public interface ISlotService
{
    Task<IEnumerable<SlotResponseDto>> GetAvailableSlotsAsync(AvailableSlotQueryDto query);
    Task<LockSlotResponseDto> LockSlotAsync(int slotId, int userId);
    Task<bool> UnlockSlotAsync(int slotId, string lockToken);
    Task<SlotResponseDto> CreateSlotAsync(CreateSlotDto createSlotDto);
    decimal CalculateDynamicPrice(decimal basePrice, int viewerCount, DateTime slotStartTime, double courtRating);
}
