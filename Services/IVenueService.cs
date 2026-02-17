using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Services;

public interface IVenueService
{
    Task<VenueResponseDto> CreateVenueAsync(CreateVenueDto createVenueDto, int ownerId);
    Task<VenueResponseDto> ApproveVenueAsync(int venueId);
    Task<VenueResponseDto> RejectVenueAsync(int venueId);
    Task<IEnumerable<VenueResponseDto>> GetAllVenuesAsync();
    Task<IEnumerable<VenueResponseDto>> GetVenuesByOwnerAsync(int ownerId);
    Task<VenueResponseDto?> GetVenueByIdAsync(int venueId);
}
