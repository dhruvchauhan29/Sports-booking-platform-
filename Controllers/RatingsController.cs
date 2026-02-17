using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly ILogger<RatingsController> _logger;

    public RatingsController(IRatingService ratingService, ILogger<RatingsController> logger)
    {
        _ratingService = ratingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<RatingResponseDto>> CreateRating([FromBody] CreateRatingDto createRatingDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var rating = await _ratingService.CreateRatingAsync(createRatingDto, userId);
            return CreatedAtAction(nameof(GetVenueRatings), new { venueId = rating.VenueId }, rating);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating");
            return StatusCode(500, new { message = "An error occurred while creating the rating" });
        }
    }

    [HttpGet("venue/{venueId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetVenueRatings(int venueId)
    {
        try
        {
            var ratings = await _ratingService.GetVenueRatingsAsync(venueId);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving venue ratings");
            return StatusCode(500, new { message = "An error occurred while retrieving ratings" });
        }
    }

    [HttpGet("court/{courtId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetCourtRatings(int courtId)
    {
        try
        {
            var ratings = await _ratingService.GetCourtRatingsAsync(courtId);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving court ratings");
            return StatusCode(500, new { message = "An error occurred while retrieving ratings" });
        }
    }

    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetUserRatings(int userId)
    {
        try
        {
            var ratings = await _ratingService.GetUserRatingsAsync(userId);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user ratings");
            return StatusCode(500, new { message = "An error occurred while retrieving ratings" });
        }
    }
}
