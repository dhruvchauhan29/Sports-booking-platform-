using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WaitlistController : ControllerBase
{
    private readonly IWaitlistService _waitlistService;
    private readonly ILogger<WaitlistController> _logger;

    public WaitlistController(IWaitlistService waitlistService, ILogger<WaitlistController> logger)
    {
        _waitlistService = waitlistService;
        _logger = logger;
    }

    [HttpPost("join")]
    public async Task<ActionResult<WaitlistResponseDto>> JoinWaitlist([FromBody] JoinWaitlistDto joinWaitlistDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var waitlist = await _waitlistService.JoinWaitlistAsync(joinWaitlistDto.GameId, userId);
            return Ok(waitlist);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining waitlist");
            return StatusCode(500, new { message = "An error occurred while joining the waitlist" });
        }
    }

    [HttpGet("game/{gameId}")]
    public async Task<ActionResult<IEnumerable<WaitlistResponseDto>>> GetGameWaitlist(int gameId)
    {
        try
        {
            var waitlist = await _waitlistService.GetGameWaitlistAsync(gameId);
            return Ok(waitlist);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving waitlist");
            return StatusCode(500, new { message = "An error occurred while retrieving the waitlist" });
        }
    }

    [HttpPost("invite")]
    [Authorize(Roles = "GameOwner,Admin")]
    public async Task<ActionResult> InviteFromWaitlist([FromBody] InviteFromWaitlistDto inviteDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            await _waitlistService.InviteFromWaitlistAsync(inviteDto.WaitlistId, userId);
            return Ok(new { message = "User invited successfully" });
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
            _logger.LogError(ex, "Error inviting from waitlist");
            return StatusCode(500, new { message = "An error occurred while inviting from the waitlist" });
        }
    }
}
