using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Services;

namespace SportsBookingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(IGameService gameService, ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "GameOwner,Admin")]
    public async Task<ActionResult<GameResponseDto>> CreateGame([FromBody] CreateGameDto createGameDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var game = await _gameService.CreateGameAsync(createGameDto, userId);
            return CreatedAtAction(nameof(GetGameById), new { id = game.GameId }, game);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            return StatusCode(500, new { message = "An error occurred while creating the game" });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GameResponseDto>> GetGameById(int id)
    {
        try
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound(new { message = "Game not found" });
            }
            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving game");
            return StatusCode(500, new { message = "An error occurred while retrieving the game" });
        }
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GameResponseDto>>> GetPublicGames()
    {
        try
        {
            var games = await _gameService.GetPublicGamesAsync();
            return Ok(games);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving public games");
            return StatusCode(500, new { message = "An error occurred while retrieving games" });
        }
    }

    [HttpGet("my-games")]
    public async Task<ActionResult<IEnumerable<GameResponseDto>>> GetMyGames()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var games = await _gameService.GetUserGamesAsync(userId);
            return Ok(games);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user games");
            return StatusCode(500, new { message = "An error occurred while retrieving games" });
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "GameOwner,Admin")]
    public async Task<ActionResult<GameResponseDto>> UpdateGameStatus(int id, [FromBody] GameStatus status)
    {
        try
        {
            var game = await _gameService.UpdateGameStatusAsync(id, status);
            return Ok(game);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating game status");
            return StatusCode(500, new { message = "An error occurred while updating game status" });
        }
    }
}
