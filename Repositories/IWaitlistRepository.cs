using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface IWaitlistRepository
{
    Task<Waitlist?> GetByIdAsync(int waitlistId);
    Task<Waitlist?> GetByGameAndUserAsync(int gameId, int userId);
    Task<IEnumerable<Waitlist>> GetByGameIdAsync(int gameId);
    Task<int> GetWaitlistCountAsync(int gameId);
    Task<Waitlist> CreateAsync(Waitlist waitlist);
    Task<Waitlist> UpdateAsync(Waitlist waitlist);
    Task DeleteAsync(Waitlist waitlist);
}
