using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Repositories;

public class CourtRepository : ICourtRepository
{
    private readonly ApplicationDbContext _context;

    public CourtRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Court?> GetByIdAsync(int courtId)
    {
        return await _context.Courts
            .Include(c => c.Venue)
            .FirstOrDefaultAsync(c => c.CourtId == courtId);
    }

    public async Task<IEnumerable<Court>> GetAllAsync()
    {
        return await _context.Courts
            .Include(c => c.Venue)
            .ToListAsync();
    }

    public async Task<IEnumerable<Court>> GetByVenueIdAsync(int venueId)
    {
        return await _context.Courts
            .Include(c => c.Venue)
            .Where(c => c.VenueId == venueId)
            .ToListAsync();
    }

    public async Task<Court> CreateAsync(Court court)
    {
        _context.Courts.Add(court);
        await _context.SaveChangesAsync();
        return court;
    }

    public async Task<Court> UpdateAsync(Court court)
    {
        _context.Courts.Update(court);
        await _context.SaveChangesAsync();
        return court;
    }

    public async Task DeleteAsync(int courtId)
    {
        var court = await _context.Courts.FindAsync(courtId);
        if (court != null)
        {
            _context.Courts.Remove(court);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasFutureBookingsAsync(int courtId)
    {
        var now = DateTime.UtcNow;
        return await _context.Slots
            .Where(s => s.CourtId == courtId && s.StartTime > now)
            .Join(_context.Bookings,
                slot => slot.SlotId,
                booking => booking.SlotId,
                (slot, booking) => booking)
            .AnyAsync(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Locked);
    }
}
