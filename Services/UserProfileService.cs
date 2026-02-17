using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetUserProfileAsync(int userId);
}

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly ApplicationDbContext _context;

    public UserProfileService(
        IUserRepository userRepository,
        IRatingRepository ratingRepository,
        ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _ratingRepository = ratingRepository;
        _context = context;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        // Get games played
        var gamesPlayed = await _context.GamePlayers
            .Where(gp => gp.UserId == userId)
            .CountAsync();

        // Get preferred sports (most played court types)
        var preferredSports = await _context.GamePlayers
            .Where(gp => gp.UserId == userId)
            .Join(
                _context.Games,
                gp => gp.GameId,
                g => g.GameId,
                (gp, g) => g.BookingId
            )
            .Where(bookingId => bookingId.HasValue)
            .Join(
                _context.Bookings,
                bookingId => bookingId!.Value,
                b => b.BookingId,
                (bookingId, b) => b.SlotId
            )
            .Join(
                _context.Slots,
                slotId => slotId,
                s => s.SlotId,
                (slotId, s) => s.CourtId
            )
            .Join(
                _context.Courts,
                courtId => courtId,
                c => c.CourtId,
                (courtId, c) => c.SportType
            )
            .GroupBy(st => st)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToListAsync();

        // Get recent reviews given by user
        var recentRatings = await _ratingRepository.GetRecentRatingsByUserAsync(userId, 5);
        var recentReviews = recentRatings.Select(r => new RatingResponseDto
        {
            RatingId = r.RatingId,
            UserId = r.UserId,
            Username = r.User.Username,
            GameId = r.GameId,
            VenueId = r.VenueId,
            CourtId = r.CourtId,
            RatedUserId = r.RatedUserId,
            RatedUsername = r.RatedUser?.Username,
            Score = r.Score,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();

        return new UserProfileDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Rating = user.Rating,
            TotalRatings = user.TotalRatings,
            GamesPlayed = gamesPlayed,
            PreferredSports = preferredSports,
            RecentReviews = recentReviews
        };
    }
}
