using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly ApplicationDbContext _context;

    public WalletRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task<WalletTransaction?> GetTransactionByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await _context.WalletTransactions
            .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);
    }

    public async Task<IEnumerable<WalletTransaction>> GetTransactionsByWalletIdAsync(int walletId)
    {
        return await _context.WalletTransactions
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Wallet> UpdateAsync(Wallet wallet)
    {
        wallet.UpdatedAt = DateTime.UtcNow;
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<WalletTransaction> CreateTransactionAsync(WalletTransaction transaction)
    {
        _context.WalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }
}
