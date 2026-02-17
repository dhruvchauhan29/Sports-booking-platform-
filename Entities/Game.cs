using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Entities;

public class Game
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public int? BookingId { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime StartTime { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Scheduled;
    public bool IsCancelled { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<GamePlayer> Players { get; set; } = new List<GamePlayer>();
    public ICollection<Waitlist> Waitlist { get; set; } = new List<Waitlist>();
}
