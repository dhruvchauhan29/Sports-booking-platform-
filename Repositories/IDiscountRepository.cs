using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface IDiscountRepository
{
    Task<Discount?> GetByIdAsync(int discountId);
    Task<IEnumerable<Discount>> GetAllAsync();
    Task<IEnumerable<Discount>> GetActiveDiscountsAsync();
    Task<IEnumerable<Discount>> GetByVenueIdAsync(int venueId);
    Task<IEnumerable<Discount>> GetByCourtIdAsync(int courtId);
    Task<Discount> CreateAsync(Discount discount);
    Task<Discount> UpdateAsync(Discount discount);
    Task DeleteAsync(int discountId);
}
