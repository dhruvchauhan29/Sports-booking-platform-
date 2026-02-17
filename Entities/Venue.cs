using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Entities;

public class Venue
{
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string SportsSupported { get; set; } = string.Empty; // Comma-separated
    public int OwnerId { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public double Rating { get; set; } = 0.0;
    public int TotalRatings { get; set; } = 0;
    
    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<Court> Courts { get; set; } = new List<Court>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
