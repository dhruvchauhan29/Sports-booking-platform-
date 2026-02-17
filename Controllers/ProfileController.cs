using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IUserProfileService userProfileService, ILogger<ProfileController> logger)
    {
        _userProfileService = userProfileService;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(int userId)
    {
        try
        {
            var profile = await _userProfileService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, new { message = "An error occurred while retrieving the profile" });
        }
    }
}
