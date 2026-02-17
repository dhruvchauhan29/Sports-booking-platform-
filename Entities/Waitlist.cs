namespace SportsBookingPlatform.Entities;

public class Waitlist
{
    public int WaitlistId { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsInvited { get; set; } = false;
    
    // Navigation properties
    public Game Game { get; set; } = null!;
    public User User { get; set; } = null!;
}
