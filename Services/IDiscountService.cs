using SportsBookingPlatform.DTOs;

namespace SportsBookingPlatform.Services;

public interface IDiscountService
{
    Task<DiscountResponseDto> CreateDiscountAsync(CreateDiscountDto createDiscountDto, int userId);
    Task<IEnumerable<DiscountResponseDto>> GetAllDiscountsAsync();
    Task<IEnumerable<DiscountResponseDto>> GetActiveDiscountsAsync();
    Task<DiscountResponseDto?> GetDiscountByIdAsync(int discountId);
}
