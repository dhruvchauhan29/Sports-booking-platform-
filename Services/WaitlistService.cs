using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public interface IWaitlistService
{
    Task<WaitlistResponseDto> JoinWaitlistAsync(int gameId, int userId);
    Task<IEnumerable<WaitlistResponseDto>> GetGameWaitlistAsync(int gameId);
    Task InviteFromWaitlistAsync(int waitlistId, int gameOwnerId);
    Task RemoveExpiredWaitlistEntriesAsync(int gameId);
}

public class WaitlistService : IWaitlistService
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IGameRepository _gameRepository;
    private const int MaxWaitlistSize = 10;

    public WaitlistService(
        IWaitlistRepository waitlistRepository,
        IGameRepository gameRepository)
    {
        _waitlistRepository = waitlistRepository;
        _gameRepository = gameRepository;
    }

    public async Task<WaitlistResponseDto> JoinWaitlistAsync(int gameId, int userId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            throw new InvalidOperationException("Game not found");
        }

        if (game.Status != GameStatus.Scheduled)
        {
            throw new InvalidOperationException("Cannot join waitlist for games that are not scheduled");
        }

        var existingEntry = await _waitlistRepository.GetByGameAndUserAsync(gameId, userId);
        if (existingEntry != null)
        {
            throw new InvalidOperationException("User is already on the waitlist");
        }

        var waitlistCount = await _waitlistRepository.GetWaitlistCountAsync(gameId);
        if (waitlistCount >= MaxWaitlistSize)
        {
            throw new InvalidOperationException($"Waitlist is full (max {MaxWaitlistSize})");
        }

        var waitlist = new Waitlist
        {
            GameId = gameId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsInvited = false
        };

        var created = await _waitlistRepository.CreateAsync(waitlist);
        var waitlistWithUser = await _waitlistRepository.GetByIdAsync(created.WaitlistId);
        
        return MapToDto(waitlistWithUser!);
    }

    public async Task<IEnumerable<WaitlistResponseDto>> GetGameWaitlistAsync(int gameId)
    {
        var waitlist = await _waitlistRepository.GetByGameIdAsync(gameId);
        return waitlist.Select(MapToDto);
    }

    public async Task InviteFromWaitlistAsync(int waitlistId, int gameOwnerId)
    {
        var waitlistEntry = await _waitlistRepository.GetByIdAsync(waitlistId);
        if (waitlistEntry == null)
        {
            throw new InvalidOperationException("Waitlist entry not found");
        }

        var game = await _gameRepository.GetByIdAsync(waitlistEntry.GameId);
        if (game == null || game.OwnerId != gameOwnerId)
        {
            throw new UnauthorizedAccessException("Not authorized to invite from this game's waitlist");
        }

        waitlistEntry.IsInvited = true;
        await _waitlistRepository.UpdateAsync(waitlistEntry);
    }

    public async Task RemoveExpiredWaitlistEntriesAsync(int gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            return;
        }

        if (game.Status == GameStatus.InProgress || game.Status == GameStatus.Completed)
        {
            var waitlistEntries = await _waitlistRepository.GetByGameIdAsync(gameId);
            foreach (var entry in waitlistEntries)
            {
                await _waitlistRepository.DeleteAsync(entry);
            }
        }
    }

    private WaitlistResponseDto MapToDto(Waitlist waitlist)
    {
        return new WaitlistResponseDto
        {
            WaitlistId = waitlist.WaitlistId,
            GameId = waitlist.GameId,
            UserId = waitlist.UserId,
            Username = waitlist.User.Username,
            UserRating = waitlist.User.Rating,
            JoinedAt = waitlist.JoinedAt,
            IsInvited = waitlist.IsInvited
        };
    }
}
