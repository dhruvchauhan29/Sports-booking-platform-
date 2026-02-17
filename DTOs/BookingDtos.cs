using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.DTOs;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int SlotId { get; set; }
    public BookingStatus Status { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
}

public class CreateBookingDto
{
    public int SlotId { get; set; }
    public string? DiscountCode { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
}

public class ConfirmBookingDto
{
    public int BookingId { get; set; }
    public string LockToken { get; set; } = string.Empty;
}

public class CancelBookingDto
{
    public int BookingId { get; set; }
    public string? CancellationReason { get; set; }
}
