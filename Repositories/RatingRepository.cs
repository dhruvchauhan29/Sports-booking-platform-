using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly ApplicationDbContext _context;

    public RatingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rating?> GetByIdAsync(int ratingId)
    {
        return await _context.Ratings
            .Include(r => r.User)
            .Include(r => r.RatedUser)
            .FirstOrDefaultAsync(r => r.RatingId == ratingId);
    }

    public async Task<Rating?> GetExistingRatingAsync(
        int userId, 
        int gameId, 
        int? venueId, 
        int? courtId, 
        int? ratedUserId)
    {
        var query = _context.Ratings
            .Where(r => r.UserId == userId && r.GameId == gameId);

        if (venueId.HasValue)
        {
            query = query.Where(r => r.VenueId == venueId.Value);
        }
        else if (courtId.HasValue)
        {
            query = query.Where(r => r.CourtId == courtId.Value);
        }
        else if (ratedUserId.HasValue)
        {
            query = query.Where(r => r.RatedUserId == ratedUserId.Value);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Rating>> GetRecentRatingsByUserAsync(int userId, int limit = 5)
    {
        return await _context.Ratings
            .Include(r => r.RatedUser)
            .Include(r => r.Venue)
            .Include(r => r.Court)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rating>> GetRatingsForVenueAsync(int venueId)
    {
        return await _context.Ratings
            .Include(r => r.User)
            .Where(r => r.VenueId == venueId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rating>> GetRatingsForCourtAsync(int courtId)
    {
        return await _context.Ratings
            .Include(r => r.User)
            .Where(r => r.CourtId == courtId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rating>> GetRatingsForUserAsync(int userId)
    {
        return await _context.Ratings
            .Include(r => r.User)
            .Where(r => r.RatedUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Rating> CreateAsync(Rating rating)
    {
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();
        return rating;
    }
}
