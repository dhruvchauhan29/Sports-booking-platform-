using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(int gameId);
    Task<IEnumerable<Game>> GetPublicGamesAsync();
    Task<IEnumerable<Game>> GetByOwnerIdAsync(int ownerId);
    Task<Game> CreateAsync(Game game);
    Task<Game> UpdateAsync(Game game);
}
