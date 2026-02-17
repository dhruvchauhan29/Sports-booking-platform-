using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.DTOs;

public class WalletResponseDto
{
    public int WalletId { get; set; }
    public int UserId { get; set; }
    public decimal Balance { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AddFundsDto
{
    public decimal Amount { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
}

public class WalletTransactionResponseDto
{
    public int TransactionId { get; set; }
    public int WalletId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
