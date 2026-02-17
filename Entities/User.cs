using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Entities;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public double Rating { get; set; } = 0.0;
    public int TotalRatings { get; set; } = 0;
    
    // Navigation properties
    public Wallet? Wallet { get; set; }
    public ICollection<Venue> Venues { get; set; } = new List<Venue>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Game> OwnedGames { get; set; } = new List<Game>();
    public ICollection<GamePlayer> GamePlayers { get; set; } = new List<GamePlayer>();
    public ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();
    public ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();
}
