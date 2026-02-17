using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface ISlotRepository
{
    Task<Slot?> GetByIdAsync(int slotId);
    Task<IEnumerable<Slot>> GetAvailableSlotsAsync(int? venueId, int? courtId, string? sportType, DateTime? startDate, DateTime? endDate);
    Task<Slot> CreateAsync(Slot slot);
    Task<Slot> UpdateAsync(Slot slot);
    Task<bool> LockSlotAsync(int slotId, string lockToken, DateTime lockedUntil);
    Task<bool> UnlockSlotAsync(int slotId, string lockToken);
    Task<IEnumerable<Slot>> GetExpiredLockedSlotsAsync();
}
