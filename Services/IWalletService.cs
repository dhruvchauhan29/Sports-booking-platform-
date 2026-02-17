using SportsBookingPlatform.DTOs;

namespace SportsBookingPlatform.Services;

public interface IWalletService
{
    Task<WalletResponseDto?> GetWalletByUserIdAsync(int userId);
    Task<WalletResponseDto> AddFundsAsync(AddFundsDto addFundsDto, int userId);
    Task<IEnumerable<WalletTransactionResponseDto>> GetTransactionHistoryAsync(int userId);
}
