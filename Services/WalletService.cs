using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<WalletResponseDto?> GetWalletByUserIdAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        return wallet == null ? null : MapToDto(wallet);
    }

    public async Task<WalletResponseDto> AddFundsAsync(AddFundsDto addFundsDto, int userId)
    {
        if (addFundsDto.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive");
        }

        // Check for idempotency
        var existingTransaction = await _walletRepository.GetTransactionByIdempotencyKeyAsync(addFundsDto.IdempotencyKey);
        if (existingTransaction != null)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            return wallet == null 
                ? throw new InvalidOperationException("Wallet not found")
                : MapToDto(wallet);
        }

        var userWallet = await _walletRepository.GetByUserIdAsync(userId);
        if (userWallet == null)
        {
            throw new InvalidOperationException("Wallet not found");
        }

        userWallet.Balance += addFundsDto.Amount;
        var updatedWallet = await _walletRepository.UpdateAsync(userWallet);

        var transaction = new WalletTransaction
        {
            WalletId = updatedWallet.WalletId,
            Type = TransactionType.Credit,
            Amount = addFundsDto.Amount,
            BalanceAfter = updatedWallet.Balance,
            Description = "Added funds",
            CreatedAt = DateTime.UtcNow,
            IdempotencyKey = addFundsDto.IdempotencyKey
        };

        await _walletRepository.CreateTransactionAsync(transaction);

        return MapToDto(updatedWallet);
    }

    public async Task<IEnumerable<WalletTransactionResponseDto>> GetTransactionHistoryAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
        {
            throw new InvalidOperationException("Wallet not found");
        }

        var transactions = await _walletRepository.GetTransactionsByWalletIdAsync(wallet.WalletId);
        return transactions.Select(t => new WalletTransactionResponseDto
        {
            TransactionId = t.TransactionId,
            WalletId = t.WalletId,
            Type = t.Type,
            Amount = t.Amount,
            BalanceAfter = t.BalanceAfter,
            Description = t.Description,
            CreatedAt = t.CreatedAt
        });
    }

    private WalletResponseDto MapToDto(Wallet wallet)
    {
        return new WalletResponseDto
        {
            WalletId = wallet.WalletId,
            UserId = wallet.UserId,
            Balance = wallet.Balance,
            UpdatedAt = wallet.UpdatedAt
        };
    }
}
