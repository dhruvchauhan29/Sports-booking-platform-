using System.ComponentModel.DataAnnotations;
using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.DTOs;

public class CreateDiscountDto
{
    [Required]
    public DiscountScope Scope { get; set; }

    public int? VenueId { get; set; }

    public int? CourtId { get; set; }

    [Required]
    [Range(0.01, 100)]
    public decimal PercentOff { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }

    [Required]
    public DateTime ValidTo { get; set; }
}

public class DiscountResponseDto
{
    public int DiscountId { get; set; }
    public string Scope { get; set; } = string.Empty;
    public int? VenueId { get; set; }
    public string? VenueName { get; set; }
    public int? CourtId { get; set; }
    public string? CourtName { get; set; }
    public decimal PercentOff { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
}
