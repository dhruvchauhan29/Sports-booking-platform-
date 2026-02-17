namespace SportsBookingPlatform.Entities;

public class Court
{
    public int CourtId { get; set; }
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
    public int SlotDurationMinutes { get; set; }
    public decimal BasePrice { get; set; }
    public string OperatingHours { get; set; } = string.Empty; // Format: "09:00-22:00"
    public bool IsActive { get; set; } = true;
    public double Rating { get; set; } = 0.0;
    public int TotalRatings { get; set; } = 0;
    
    // Navigation properties
    public Venue Venue { get; set; } = null!;
    public ICollection<Slot> Slots { get; set; } = new List<Slot>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
