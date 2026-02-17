using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SlotsController : ControllerBase
{
    private readonly ISlotService _slotService;
    private readonly ILogger<SlotsController> _logger;

    public SlotsController(ISlotService slotService, ILogger<SlotsController> logger)
    {
        _slotService = slotService;
        _logger = logger;
    }

    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SlotResponseDto>>> GetAvailableSlots(
        [FromQuery] int? venueId,
        [FromQuery] int? courtId,
        [FromQuery] string? sportType,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        try
        {
            var query = new AvailableSlotQueryDto
            {
                VenueId = venueId,
                CourtId = courtId,
                SportType = sportType,
                StartDate = startDate,
                EndDate = endDate
            };

            var slots = await _slotService.GetAvailableSlotsAsync(query);
            return Ok(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available slots");
            return StatusCode(500, new { message = "An error occurred while retrieving slots" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "VenueOwner,Admin")]
    public async Task<ActionResult<SlotResponseDto>> CreateSlot([FromBody] CreateSlotDto createSlotDto)
    {
        try
        {
            var slot = await _slotService.CreateSlotAsync(createSlotDto);
            return CreatedAtAction(nameof(GetAvailableSlots), new { slotId = slot.SlotId }, slot);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating slot");
            return StatusCode(500, new { message = "An error occurred while creating the slot" });
        }
    }

    [HttpPost("{slotId}/lock")]
    public async Task<ActionResult<LockSlotResponseDto>> LockSlot(int slotId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var lockResponse = await _slotService.LockSlotAsync(slotId, userId);
            return Ok(lockResponse);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking slot");
            return StatusCode(500, new { message = "An error occurred while locking the slot" });
        }
    }
}
