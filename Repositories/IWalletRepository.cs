using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(int userId);
    Task<WalletTransaction?> GetTransactionByIdempotencyKeyAsync(string idempotencyKey);
    Task<IEnumerable<WalletTransaction>> GetTransactionsByWalletIdAsync(int walletId);
    Task<Wallet> UpdateAsync(Wallet wallet);
    Task<WalletTransaction> CreateTransactionAsync(WalletTransaction transaction);
}
