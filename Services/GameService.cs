using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public interface IGameService
{
    Task<GameResponseDto> CreateGameAsync(CreateGameDto createGameDto, int ownerId);
    Task<GameResponseDto?> GetGameByIdAsync(int gameId);
    Task<IEnumerable<GameResponseDto>> GetPublicGamesAsync();
    Task<IEnumerable<GameResponseDto>> GetUserGamesAsync(int userId);
    Task<GameResponseDto> UpdateGameStatusAsync(int gameId, GameStatus status);
}

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<GameResponseDto> CreateGameAsync(CreateGameDto createGameDto, int ownerId)
    {
        var game = new Game
        {
            Name = createGameDto.Name,
            Description = createGameDto.Description,
            OwnerId = ownerId,
            BookingId = createGameDto.BookingId,
            MinPlayers = createGameDto.MinPlayers,
            MaxPlayers = createGameDto.MaxPlayers,
            IsPublic = createGameDto.IsPublic,
            StartTime = createGameDto.StartTime,
            Status = GameStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        var createdGame = await _gameRepository.CreateAsync(game);
        var gameWithRelations = await _gameRepository.GetByIdAsync(createdGame.GameId);
        
        return MapToDto(gameWithRelations!);
    }

    public async Task<GameResponseDto?> GetGameByIdAsync(int gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        return game == null ? null : MapToDto(game);
    }

    public async Task<IEnumerable<GameResponseDto>> GetPublicGamesAsync()
    {
        var games = await _gameRepository.GetPublicGamesAsync();
        return games.Select(MapToDto);
    }

    public async Task<IEnumerable<GameResponseDto>> GetUserGamesAsync(int userId)
    {
        var games = await _gameRepository.GetByOwnerIdAsync(userId);
        return games.Select(MapToDto);
    }

    public async Task<GameResponseDto> UpdateGameStatusAsync(int gameId, GameStatus status)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            throw new InvalidOperationException("Game not found");
        }

        game.Status = status;
        if (status == GameStatus.Cancelled)
        {
            game.IsCancelled = true;
        }

        var updatedGame = await _gameRepository.UpdateAsync(game);
        return MapToDto(updatedGame);
    }

    private GameResponseDto MapToDto(Game game)
    {
        return new GameResponseDto
        {
            GameId = game.GameId,
            Name = game.Name,
            Description = game.Description,
            OwnerId = game.OwnerId,
            OwnerName = game.Owner.Username,
            BookingId = game.BookingId,
            MinPlayers = game.MinPlayers,
            MaxPlayers = game.MaxPlayers,
            IsPublic = game.IsPublic,
            StartTime = game.StartTime,
            Status = game.Status,
            CurrentPlayerCount = game.Players.Count,
            CreatedAt = game.CreatedAt
        };
    }
}
