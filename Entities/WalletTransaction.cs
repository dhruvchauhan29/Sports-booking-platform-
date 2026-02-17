using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Entities;

public class WalletTransaction
{
    public int TransactionId { get; set; }
    public int WalletId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? BookingId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? IdempotencyKey { get; set; }
    
    // Navigation properties
    public Wallet Wallet { get; set; } = null!;
}
