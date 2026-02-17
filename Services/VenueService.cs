using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;
    private readonly IUserRepository _userRepository;

    public VenueService(IVenueRepository venueRepository, IUserRepository userRepository)
    {
        _venueRepository = venueRepository;
        _userRepository = userRepository;
    }

    public async Task<VenueResponseDto> CreateVenueAsync(CreateVenueDto createVenueDto, int ownerId)
    {
        var owner = await _userRepository.GetByIdAsync(ownerId);
        if (owner == null)
        {
            throw new InvalidOperationException("Owner not found");
        }

        if (owner.Role != UserRole.VenueOwner && owner.Role != UserRole.Admin)
        {
            throw new UnauthorizedAccessException("Only venue owners can create venues");
        }

        var venue = new Venue
        {
            Name = createVenueDto.Name,
            Address = createVenueDto.Address,
            SportsSupported = createVenueDto.SportsSupported,
            OwnerId = ownerId,
            ApprovalStatus = ApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var createdVenue = await _venueRepository.CreateAsync(venue);
        
        return MapToDto(createdVenue);
    }

    public async Task<VenueResponseDto> ApproveVenueAsync(int venueId)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId);
        if (venue == null)
        {
            throw new InvalidOperationException("Venue not found");
        }

        venue.ApprovalStatus = ApprovalStatus.Approved;
        var updatedVenue = await _venueRepository.UpdateAsync(venue);
        
        return MapToDto(updatedVenue);
    }

    public async Task<VenueResponseDto> RejectVenueAsync(int venueId)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId);
        if (venue == null)
        {
            throw new InvalidOperationException("Venue not found");
        }

        venue.ApprovalStatus = ApprovalStatus.Rejected;
        var updatedVenue = await _venueRepository.UpdateAsync(venue);
        
        return MapToDto(updatedVenue);
    }

    public async Task<IEnumerable<VenueResponseDto>> GetAllVenuesAsync()
    {
        var venues = await _venueRepository.GetAllAsync();
        return venues.Select(MapToDto);
    }

    public async Task<IEnumerable<VenueResponseDto>> GetVenuesByOwnerAsync(int ownerId)
    {
        var venues = await _venueRepository.GetByOwnerIdAsync(ownerId);
        return venues.Select(MapToDto);
    }

    public async Task<VenueResponseDto?> GetVenueByIdAsync(int venueId)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId);
        return venue == null ? null : MapToDto(venue);
    }

    private VenueResponseDto MapToDto(Venue venue)
    {
        return new VenueResponseDto
        {
            VenueId = venue.VenueId,
            Name = venue.Name,
            Address = venue.Address,
            SportsSupported = venue.SportsSupported,
            OwnerId = venue.OwnerId,
            OwnerName = venue.Owner?.Username ?? "",
            ApprovalStatus = venue.ApprovalStatus.ToString(),
            CreatedAt = venue.CreatedAt,
            Rating = venue.Rating,
            TotalRatings = venue.TotalRatings
        };
    }
}
