using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Repositories;

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(int venueId);
    Task<IEnumerable<Venue>> GetAllAsync();
    Task<IEnumerable<Venue>> GetByOwnerIdAsync(int ownerId);
    Task<IEnumerable<Venue>> GetByApprovalStatusAsync(ApprovalStatus status);
    Task<Venue> CreateAsync(Venue venue);
    Task<Venue> UpdateAsync(Venue venue);
    Task DeleteAsync(int venueId);
}
