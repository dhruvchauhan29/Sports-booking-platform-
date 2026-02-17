using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletsController> _logger;

    public WalletsController(IWalletService walletService, ILogger<WalletsController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    [HttpGet("balance")]
    public async Task<ActionResult<WalletResponseDto>> GetBalance()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            if (wallet == null)
            {
                return NotFound(new { message = "Wallet not found" });
            }
            return Ok(wallet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wallet balance");
            return StatusCode(500, new { message = "An error occurred while retrieving the wallet" });
        }
    }

    [HttpPost("add-funds")]
    public async Task<ActionResult<WalletResponseDto>> AddFunds([FromBody] AddFundsDto addFundsDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var wallet = await _walletService.AddFundsAsync(addFundsDto, userId);
            return Ok(wallet);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding funds");
            return StatusCode(500, new { message = "An error occurred while adding funds" });
        }
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<IEnumerable<WalletTransactionResponseDto>>> GetTransactionHistory()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var transactions = await _walletService.GetTransactionHistoryAsync(userId);
            return Ok(transactions);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction history");
            return StatusCode(500, new { message = "An error occurred while retrieving transactions" });
        }
    }
}
