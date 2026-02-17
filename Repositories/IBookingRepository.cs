using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int bookingId);
    Task<Booking?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking> UpdateAsync(Booking booking);
}
