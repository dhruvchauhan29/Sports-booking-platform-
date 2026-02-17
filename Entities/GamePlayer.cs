namespace SportsBookingPlatform.Entities;

public class GamePlayer
{
    public int GamePlayerId { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Game Game { get; set; } = null!;
    public User User { get; set; } = null!;
}
