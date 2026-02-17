using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public class GameRepository : IGameRepository
{
    private readonly ApplicationDbContext _context;

    public GameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetByIdAsync(int gameId)
    {
        return await _context.Games
            .Include(g => g.Owner)
            .Include(g => g.Players)
            .ThenInclude(p => p.User)
            .Include(g => g.Waitlist)
            .ThenInclude(w => w.User)
            .FirstOrDefaultAsync(g => g.GameId == gameId);
    }

    public async Task<IEnumerable<Game>> GetPublicGamesAsync()
    {
        return await _context.Games
            .Include(g => g.Owner)
            .Include(g => g.Players)
            .Where(g => g.IsPublic && !g.IsCancelled)
            .OrderBy(g => g.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Games
            .Include(g => g.Players)
            .ThenInclude(p => p.User)
            .Where(g => g.OwnerId == ownerId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<Game> CreateAsync(Game game)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<Game> UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
        return game;
    }
}
