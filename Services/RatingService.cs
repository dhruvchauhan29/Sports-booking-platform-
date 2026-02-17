using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public interface IRatingService
{
    Task<RatingResponseDto> CreateRatingAsync(CreateRatingDto createRatingDto, int userId);
    Task<IEnumerable<RatingResponseDto>> GetVenueRatingsAsync(int venueId);
    Task<IEnumerable<RatingResponseDto>> GetCourtRatingsAsync(int courtId);
    Task<IEnumerable<RatingResponseDto>> GetUserRatingsAsync(int userId);
}

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly ICourtRepository _courtRepository;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;

    public RatingService(
        IRatingRepository ratingRepository,
        IGameRepository gameRepository,
        IVenueRepository venueRepository,
        ICourtRepository courtRepository,
        IUserRepository userRepository,
        ApplicationDbContext context)
    {
        _ratingRepository = ratingRepository;
        _gameRepository = gameRepository;
        _venueRepository = venueRepository;
        _courtRepository = courtRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<RatingResponseDto> CreateRatingAsync(CreateRatingDto createRatingDto, int userId)
    {
        // Validate game is completed
        var game = await _gameRepository.GetByIdAsync(createRatingDto.GameId);
        if (game == null)
        {
            throw new InvalidOperationException("Game not found");
        }

        if (game.Status != GameStatus.Completed)
        {
            throw new InvalidOperationException("Can only rate after game is completed");
        }

        // Validate score
        if (createRatingDto.Score < 1 || createRatingDto.Score > 5)
        {
            throw new InvalidOperationException("Score must be between 1 and 5");
        }

        // Check for existing rating
        var existingRating = await _ratingRepository.GetExistingRatingAsync(
            userId,
            createRatingDto.GameId,
            createRatingDto.VenueId,
            createRatingDto.CourtId,
            createRatingDto.RatedUserId
        );

        if (existingRating != null)
        {
            throw new InvalidOperationException("You have already rated this entity for this game");
        }

        var rating = new Rating
        {
            UserId = userId,
            GameId = createRatingDto.GameId,
            VenueId = createRatingDto.VenueId,
            CourtId = createRatingDto.CourtId,
            RatedUserId = createRatingDto.RatedUserId,
            Score = createRatingDto.Score,
            Comment = createRatingDto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        var createdRating = await _ratingRepository.CreateAsync(rating);

        // Update aggregated ratings
        await UpdateAggregatedRatingsAsync(createRatingDto);

        var ratingWithRelations = await _ratingRepository.GetByIdAsync(createdRating.RatingId);
        return MapToDto(ratingWithRelations!);
    }

    public async Task<IEnumerable<RatingResponseDto>> GetVenueRatingsAsync(int venueId)
    {
        var ratings = await _ratingRepository.GetRatingsForVenueAsync(venueId);
        return ratings.Select(MapToDto);
    }

    public async Task<IEnumerable<RatingResponseDto>> GetCourtRatingsAsync(int courtId)
    {
        var ratings = await _ratingRepository.GetRatingsForCourtAsync(courtId);
        return ratings.Select(MapToDto);
    }

    public async Task<IEnumerable<RatingResponseDto>> GetUserRatingsAsync(int userId)
    {
        var ratings = await _ratingRepository.GetRatingsForUserAsync(userId);
        return ratings.Select(MapToDto);
    }

    private async Task UpdateAggregatedRatingsAsync(CreateRatingDto createRatingDto)
    {
        if (createRatingDto.VenueId.HasValue)
        {
            var venue = await _venueRepository.GetByIdAsync(createRatingDto.VenueId.Value);
            if (venue != null)
            {
                var ratings = await _ratingRepository.GetRatingsForVenueAsync(createRatingDto.VenueId.Value);
                venue.TotalRatings = ratings.Count();
                venue.Rating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                await _venueRepository.UpdateAsync(venue);
            }
        }

        if (createRatingDto.CourtId.HasValue)
        {
            var court = await _courtRepository.GetByIdAsync(createRatingDto.CourtId.Value);
            if (court != null)
            {
                var ratings = await _ratingRepository.GetRatingsForCourtAsync(createRatingDto.CourtId.Value);
                court.TotalRatings = ratings.Count();
                court.Rating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                await _courtRepository.UpdateAsync(court);
            }
        }

        if (createRatingDto.RatedUserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(createRatingDto.RatedUserId.Value);
            if (user != null)
            {
                var ratings = await _ratingRepository.GetRatingsForUserAsync(createRatingDto.RatedUserId.Value);
                user.TotalRatings = ratings.Count();
                user.Rating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                await _userRepository.UpdateAsync(user);
            }
        }
    }

    private RatingResponseDto MapToDto(Rating rating)
    {
        return new RatingResponseDto
        {
            RatingId = rating.RatingId,
            UserId = rating.UserId,
            Username = rating.User.Username,
            GameId = rating.GameId,
            VenueId = rating.VenueId,
            CourtId = rating.CourtId,
            RatedUserId = rating.RatedUserId,
            RatedUsername = rating.RatedUser?.Username,
            Score = rating.Score,
            Comment = rating.Comment,
            CreatedAt = rating.CreatedAt
        };
    }
}
