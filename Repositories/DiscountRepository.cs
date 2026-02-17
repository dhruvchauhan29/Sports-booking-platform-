using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly ApplicationDbContext _context;

    public DiscountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Discount?> GetByIdAsync(int discountId)
    {
        return await _context.Discounts
            .Include(d => d.Venue)
            .Include(d => d.Court)
            .FirstOrDefaultAsync(d => d.DiscountId == discountId);
    }

    public async Task<IEnumerable<Discount>> GetAllAsync()
    {
        return await _context.Discounts
            .Include(d => d.Venue)
            .Include(d => d.Court)
            .ToListAsync();
    }

    public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Discounts
            .Include(d => d.Venue)
            .Include(d => d.Court)
            .Where(d => d.IsActive && d.ValidFrom <= now && d.ValidTo >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Discount>> GetByVenueIdAsync(int venueId)
    {
        return await _context.Discounts
            .Include(d => d.Venue)
            .Where(d => d.VenueId == venueId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Discount>> GetByCourtIdAsync(int courtId)
    {
        return await _context.Discounts
            .Include(d => d.Court)
            .Where(d => d.CourtId == courtId)
            .ToListAsync();
    }

    public async Task<Discount> CreateAsync(Discount discount)
    {
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task<Discount> UpdateAsync(Discount discount)
    {
        _context.Discounts.Update(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task DeleteAsync(int discountId)
    {
        var discount = await _context.Discounts.FindAsync(discountId);
        if (discount != null)
        {
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }
    }
}
