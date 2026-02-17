using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/venues/{venueId}/courts")]
[Authorize]
public class CourtsController : ControllerBase
{
    private readonly ICourtService _courtService;
    private readonly ILogger<CourtsController> _logger;

    public CourtsController(ICourtService courtService, ILogger<CourtsController> logger)
    {
        _courtService = courtService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "VenueOwner,Admin")]
    public async Task<ActionResult<CourtResponseDto>> CreateCourt(int venueId, [FromBody] CreateCourtDto createCourtDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            createCourtDto.VenueId = venueId;
            var court = await _courtService.CreateCourtAsync(createCourtDto, userId);
            return CreatedAtRoute("GetCourtById", new { venueId, id = court.CourtId }, court);
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
            _logger.LogError(ex, "Error creating court");
            return StatusCode(500, new { message = "An error occurred while creating the court" });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CourtResponseDto>>> GetCourtsByVenue(int venueId)
    {
        try
        {
            var courts = await _courtService.GetCourtsByVenueAsync(venueId);
            return Ok(courts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courts");
            return StatusCode(500, new { message = "An error occurred while retrieving courts" });
        }
    }

    [HttpGet("{id}", Name = "GetCourtById")]
    [AllowAnonymous]
    public async Task<ActionResult<CourtResponseDto>> GetCourtById(int venueId, int id)
    {
        try
        {
            var court = await _courtService.GetCourtByIdAsync(id);
            if (court == null || court.VenueId != venueId)
            {
                return NotFound(new { message = "Court not found" });
            }
            return Ok(court);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving court");
            return StatusCode(500, new { message = "An error occurred while retrieving the court" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "VenueOwner,Admin")]
    public async Task<ActionResult<CourtResponseDto>> UpdateCourt(int venueId, int id, [FromBody] UpdateCourtDto updateCourtDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var court = await _courtService.UpdateCourtAsync(id, updateCourtDto, userId);
            return Ok(court);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating court");
            return StatusCode(500, new { message = "An error occurred while updating the court" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "VenueOwner,Admin")]
    public async Task<ActionResult> DeleteCourt(int venueId, int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            await _courtService.DeleteCourtAsync(id, userId);
            return NoContent();
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
            _logger.LogError(ex, "Error deleting court");
            return StatusCode(500, new { message = "An error occurred while deleting the court" });
        }
    }
}
