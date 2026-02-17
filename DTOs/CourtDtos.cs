using System.ComponentModel.DataAnnotations;

namespace SportsBookingPlatform.DTOs;

public class CreateCourtDto
{
    [Required]
    public int VenueId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string SportType { get; set; } = string.Empty;

    [Required]
    [Range(15, 180)]
    public int SlotDurationMinutes { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }

    [Required]
    public string OperatingHours { get; set; } = string.Empty; // Format: "09:00-22:00"
}

public class UpdateCourtDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? BasePrice { get; set; }

    public string? OperatingHours { get; set; }

    public bool? IsActive { get; set; }
}

public class CourtResponseDto
{
    public int CourtId { get; set; }
    public int VenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
    public int SlotDurationMinutes { get; set; }
    public decimal BasePrice { get; set; }
    public string OperatingHours { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public double Rating { get; set; }
    public int TotalRatings { get; set; }
}
