using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;
    private readonly ILogger<VenuesController> _logger;

    public VenuesController(IVenueService venueService, ILogger<VenuesController> logger)
    {
        _venueService = venueService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "VenueOwner,Admin")]
    public async Task<ActionResult<VenueResponseDto>> CreateVenue([FromBody] CreateVenueDto createVenueDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var venue = await _venueService.CreateVenueAsync(createVenueDto, userId);
            return CreatedAtAction(nameof(GetVenueById), new { id = venue.VenueId }, venue);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating venue");
            return StatusCode(500, new { message = "An error occurred while creating the venue" });
        }
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VenueResponseDto>> ApproveVenue(int id)
    {
        try
        {
            var venue = await _venueService.ApproveVenueAsync(id);
            return Ok(venue);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving venue");
            return StatusCode(500, new { message = "An error occurred while approving the venue" });
        }
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VenueResponseDto>> RejectVenue(int id)
    {
        try
        {
            var venue = await _venueService.RejectVenueAsync(id);
            return Ok(venue);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting venue");
            return StatusCode(500, new { message = "An error occurred while rejecting the venue" });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<VenueResponseDto>>> GetAllVenues()
    {
        try
        {
            var venues = await _venueService.GetAllVenuesAsync();
            return Ok(venues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving venues");
            return StatusCode(500, new { message = "An error occurred while retrieving venues" });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<VenueResponseDto>> GetVenueById(int id)
    {
        try
        {
            var venue = await _venueService.GetVenueByIdAsync(id);
            if (venue == null)
            {
                return NotFound(new { message = "Venue not found" });
            }
            return Ok(venue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving venue");
            return StatusCode(500, new { message = "An error occurred while retrieving the venue" });
        }
    }

    [HttpGet("owner/my-venues")]
    [Authorize(Roles = "VenueOwner,Admin")]
    public async Task<ActionResult<IEnumerable<VenueResponseDto>>> GetMyVenues()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var venues = await _venueService.GetVenuesByOwnerAsync(userId);
            return Ok(venues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving owner venues");
            return StatusCode(500, new { message = "An error occurred while retrieving venues" });
        }
    }
}
