using System.ComponentModel.DataAnnotations;

namespace SportsBookingPlatform.DTOs;

public class CreateVenueDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string SportsSupported { get; set; } = string.Empty; // Comma-separated
}

public class VenueResponseDto
{
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string SportsSupported { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public double Rating { get; set; }
    public int TotalRatings { get; set; }
}
