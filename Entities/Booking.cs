using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Entities;

public class Booking
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int SlotId { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal FinalPrice { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? IdempotencyKey { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Slot Slot { get; set; } = null!;
}
