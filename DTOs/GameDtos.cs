using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.DTOs;

public class GameResponseDto
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int? BookingId { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public bool IsPublic { get; set; }
    public DateTime StartTime { get; set; }
    public GameStatus Status { get; set; }
    public int CurrentPlayerCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateGameDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? BookingId { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime StartTime { get; set; }
}

public class WaitlistResponseDto
{
    public int WaitlistId { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public double UserRating { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsInvited { get; set; }
}

public class JoinWaitlistDto
{
    public int GameId { get; set; }
}

public class InviteFromWaitlistDto
{
    public int WaitlistId { get; set; }
}

public class RatingResponseDto
{
    public int RatingId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int GameId { get; set; }
    public int? VenueId { get; set; }
    public int? CourtId { get; set; }
    public int? RatedUserId { get; set; }
    public string? RatedUsername { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRatingDto
{
    public int GameId { get; set; }
    public int? VenueId { get; set; }
    public int? CourtId { get; set; }
    public int? RatedUserId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}

public class UserProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int TotalRatings { get; set; }
    public int GamesPlayed { get; set; }
    public List<string> PreferredSports { get; set; } = new();
    public List<RatingResponseDto> RecentReviews { get; set; } = new();
}
