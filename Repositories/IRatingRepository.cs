using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface IRatingRepository
{
    Task<Rating?> GetByIdAsync(int ratingId);
    Task<Rating?> GetExistingRatingAsync(int userId, int gameId, int? venueId, int? courtId, int? ratedUserId);
    Task<IEnumerable<Rating>> GetRecentRatingsByUserAsync(int userId, int limit = 5);
    Task<IEnumerable<Rating>> GetRatingsForVenueAsync(int venueId);
    Task<IEnumerable<Rating>> GetRatingsForCourtAsync(int courtId);
    Task<IEnumerable<Rating>> GetRatingsForUserAsync(int userId);
    Task<Rating> CreateAsync(Rating rating);
}
