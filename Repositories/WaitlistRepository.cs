using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public class WaitlistRepository : IWaitlistRepository
{
    private readonly ApplicationDbContext _context;

    public WaitlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Waitlist?> GetByIdAsync(int waitlistId)
    {
        return await _context.Waitlists
            .Include(w => w.User)
            .Include(w => w.Game)
            .FirstOrDefaultAsync(w => w.WaitlistId == waitlistId);
    }

    public async Task<Waitlist?> GetByGameAndUserAsync(int gameId, int userId)
    {
        return await _context.Waitlists
            .FirstOrDefaultAsync(w => w.GameId == gameId && w.UserId == userId);
    }

    public async Task<IEnumerable<Waitlist>> GetByGameIdAsync(int gameId)
    {
        return await _context.Waitlists
            .Include(w => w.User)
            .Where(w => w.GameId == gameId)
            .OrderByDescending(w => w.User.Rating)
            .ThenBy(w => w.JoinedAt)
            .ToListAsync();
    }

    public async Task<int> GetWaitlistCountAsync(int gameId)
    {
        return await _context.Waitlists
            .CountAsync(w => w.GameId == gameId);
    }

    public async Task<Waitlist> CreateAsync(Waitlist waitlist)
    {
        _context.Waitlists.Add(waitlist);
        await _context.SaveChangesAsync();
        return waitlist;
    }

    public async Task<Waitlist> UpdateAsync(Waitlist waitlist)
    {
        _context.Waitlists.Update(waitlist);
        await _context.SaveChangesAsync();
        return waitlist;
    }

    public async Task DeleteAsync(Waitlist waitlist)
    {
        _context.Waitlists.Remove(waitlist);
        await _context.SaveChangesAsync();
    }
}
