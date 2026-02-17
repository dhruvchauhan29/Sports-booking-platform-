using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.DTOs;

public class SlotResponseDto
{
    public int SlotId { get; set; }
    public int CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public decimal CurrentPrice { get; set; }
    public int ViewerCount { get; set; }
}

public class AvailableSlotQueryDto
{
    public int? VenueId { get; set; }
    public int? CourtId { get; set; }
    public string? SportType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CreateSlotDto
{
    public int CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class LockSlotDto
{
    public int SlotId { get; set; }
}

public class LockSlotResponseDto
{
    public int SlotId { get; set; }
    public string LockToken { get; set; } = string.Empty;
    public DateTime LockedUntil { get; set; }
}
