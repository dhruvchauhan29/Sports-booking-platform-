using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var booking = await _bookingService.CreateBookingAsync(createBookingDto, userId);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, new { message = "An error occurred while creating the booking" });
        }
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<BookingResponseDto>> ConfirmBooking(int id, [FromBody] ConfirmBookingDto confirmBookingDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            confirmBookingDto.BookingId = id;
            var booking = await _bookingService.ConfirmBookingAsync(confirmBookingDto, userId);
            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming booking");
            return StatusCode(500, new { message = "An error occurred while confirming the booking" });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<BookingResponseDto>> CancelBooking(int id, [FromBody] CancelBookingDto? cancelBookingDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var dto = cancelBookingDto ?? new CancelBookingDto();
            dto.BookingId = id;
            var booking = await _bookingService.CancelBookingAsync(dto, userId);
            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking");
            return StatusCode(500, new { message = "An error occurred while cancelling the booking" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetUserBookings()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user bookings");
            return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponseDto>> GetBookingById(int id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }
            return Ok(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking");
            return StatusCode(500, new { message = "An error occurred while retrieving the booking" });
        }
    }
}
