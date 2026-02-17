using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountService _discountService;
    private readonly ILogger<DiscountsController> _logger;

    public DiscountsController(IDiscountService discountService, ILogger<DiscountsController> logger)
    {
        _discountService = discountService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "VenueOwner,Admin")]
    public async Task<ActionResult<DiscountResponseDto>> CreateDiscount([FromBody] CreateDiscountDto createDiscountDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var discount = await _discountService.CreateDiscountAsync(createDiscountDto, userId);
            return CreatedAtAction(nameof(GetDiscountById), new { id = discount.DiscountId }, discount);
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
            _logger.LogError(ex, "Error creating discount");
            return StatusCode(500, new { message = "An error occurred while creating the discount" });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DiscountResponseDto>>> GetAllDiscounts([FromQuery] bool activeOnly = false)
    {
        try
        {
            var discounts = activeOnly 
                ? await _discountService.GetActiveDiscountsAsync()
                : await _discountService.GetAllDiscountsAsync();
            return Ok(discounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving discounts");
            return StatusCode(500, new { message = "An error occurred while retrieving discounts" });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<DiscountResponseDto>> GetDiscountById(int id)
    {
        try
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            if (discount == null)
            {
                return NotFound(new { message = "Discount not found" });
            }
            return Ok(discount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving discount");
            return StatusCode(500, new { message = "An error occurred while retrieving the discount" });
        }
    }
}
