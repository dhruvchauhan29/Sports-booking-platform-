namespace SportsBookingPlatform.Entities;

public class Rating
{
    public int RatingId { get; set; }
    public int UserId { get; set; } // User who gave the rating
    public int GameId { get; set; }
    public int? VenueId { get; set; }
    public int? CourtId { get; set; }
    public int? RatedUserId { get; set; } // Player being rated
    public int Score { get; set; } // 1-5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!; // Rater
    public User? RatedUser { get; set; } // Rated player
    public Venue? Venue { get; set; }
    public Court? Court { get; set; }
}
