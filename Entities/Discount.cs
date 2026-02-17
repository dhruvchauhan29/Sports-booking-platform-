using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Entities;

public class Discount
{
    public int DiscountId { get; set; }
    public DiscountScope Scope { get; set; }
    public int? VenueId { get; set; }
    public int? CourtId { get; set; }
    public decimal PercentOff { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Venue? Venue { get; set; }
    public Court? Court { get; set; }
}
