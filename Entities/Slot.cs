using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Entities;

public class Slot
{
    public int SlotId { get; set; }
    public int CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Available;
    public decimal CurrentPrice { get; set; }
    public int ViewerCount { get; set; } = 0;
    public DateTime? LockedUntil { get; set; }
    public string? LockToken { get; set; }
    
    // Navigation properties
    public Court Court { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
