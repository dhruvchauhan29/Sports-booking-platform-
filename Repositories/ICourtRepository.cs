using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface ICourtRepository
{
    Task<Court?> GetByIdAsync(int courtId);
    Task<IEnumerable<Court>> GetAllAsync();
    Task<IEnumerable<Court>> GetByVenueIdAsync(int venueId);
    Task<Court> CreateAsync(Court court);
    Task<Court> UpdateAsync(Court court);
    Task DeleteAsync(int courtId);
    Task<bool> HasFutureBookingsAsync(int courtId);
}
