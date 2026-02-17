using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Repositories;

public class SlotRepository : ISlotRepository
{
    private readonly ApplicationDbContext _context;

    public SlotRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Slot?> GetByIdAsync(int slotId)
    {
        return await _context.Slots
            .Include(s => s.Court)
            .ThenInclude(c => c.Venue)
            .FirstOrDefaultAsync(s => s.SlotId == slotId);
    }

    public async Task<IEnumerable<Slot>> GetAvailableSlotsAsync(
        int? venueId, 
        int? courtId, 
        string? sportType, 
        DateTime? startDate, 
        DateTime? endDate)
    {
        var query = _context.Slots
            .Include(s => s.Court)
            .ThenInclude(c => c.Venue)
            .Where(s => s.Status == BookingStatus.Available || 
                       (s.Status == BookingStatus.Locked && s.LockedUntil < DateTime.UtcNow));

        if (venueId.HasValue)
        {
            query = query.Where(s => s.Court.VenueId == venueId.Value);
        }

        if (courtId.HasValue)
        {
            query = query.Where(s => s.CourtId == courtId.Value);
        }

        if (!string.IsNullOrEmpty(sportType))
        {
            query = query.Where(s => s.Court.SportType == sportType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(s => s.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.EndTime <= endDate.Value);
        }

        return await query.OrderBy(s => s.StartTime).ToListAsync();
    }

    public async Task<Slot> CreateAsync(Slot slot)
    {
        _context.Slots.Add(slot);
        await _context.SaveChangesAsync();
        return slot;
    }

    public async Task<Slot> UpdateAsync(Slot slot)
    {
        _context.Slots.Update(slot);
        await _context.SaveChangesAsync();
        return slot;
    }

    public async Task<bool> LockSlotAsync(int slotId, string lockToken, DateTime lockedUntil)
    {
        var slot = await _context.Slots.FindAsync(slotId);
        if (slot == null || (slot.Status != BookingStatus.Available && 
                            (slot.Status != BookingStatus.Locked || slot.LockedUntil >= DateTime.UtcNow)))
        {
            return false;
        }

        slot.Status = BookingStatus.Locked;
        slot.LockToken = lockToken;
        slot.LockedUntil = lockedUntil;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlockSlotAsync(int slotId, string lockToken)
    {
        var slot = await _context.Slots.FindAsync(slotId);
        if (slot == null || slot.Status != BookingStatus.Locked || slot.LockToken != lockToken)
        {
            return false;
        }

        slot.Status = BookingStatus.Available;
        slot.LockToken = null;
        slot.LockedUntil = null;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Slot>> GetExpiredLockedSlotsAsync()
    {
        return await _context.Slots
            .Where(s => s.Status == BookingStatus.Locked && s.LockedUntil < DateTime.UtcNow)
            .ToListAsync();
    }
}
