using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public class CourtService : ICourtService
{
    private readonly ICourtRepository _courtRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IUserRepository _userRepository;

    public CourtService(ICourtRepository courtRepository, IVenueRepository venueRepository, IUserRepository userRepository)
    {
        _courtRepository = courtRepository;
        _venueRepository = venueRepository;
        _userRepository = userRepository;
    }

    public async Task<CourtResponseDto> CreateCourtAsync(CreateCourtDto createCourtDto, int userId)
    {
        var venue = await _venueRepository.GetByIdAsync(createCourtDto.VenueId);
        if (venue == null)
        {
            throw new InvalidOperationException("Venue not found");
        }

        if (venue.OwnerId != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only the venue owner or admin can add courts");
            }
        }

        if (venue.ApprovalStatus != ApprovalStatus.Approved)
        {
            throw new InvalidOperationException("Can only add courts to approved venues");
        }

        var court = new Court
        {
            VenueId = createCourtDto.VenueId,
            Name = createCourtDto.Name,
            SportType = createCourtDto.SportType,
            SlotDurationMinutes = createCourtDto.SlotDurationMinutes,
            BasePrice = createCourtDto.BasePrice,
            OperatingHours = createCourtDto.OperatingHours,
            IsActive = true
        };

        var createdCourt = await _courtRepository.CreateAsync(court);
        return MapToDto(createdCourt);
    }

    public async Task<CourtResponseDto> UpdateCourtAsync(int courtId, UpdateCourtDto updateCourtDto, int userId)
    {
        var court = await _courtRepository.GetByIdAsync(courtId);
        if (court == null)
        {
            throw new InvalidOperationException("Court not found");
        }

        if (court.Venue.OwnerId != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only the venue owner or admin can update courts");
            }
        }

        if (updateCourtDto.Name != null) court.Name = updateCourtDto.Name;
        if (updateCourtDto.BasePrice.HasValue) court.BasePrice = updateCourtDto.BasePrice.Value;
        if (updateCourtDto.OperatingHours != null) court.OperatingHours = updateCourtDto.OperatingHours;
        if (updateCourtDto.IsActive.HasValue) court.IsActive = updateCourtDto.IsActive.Value;

        var updatedCourt = await _courtRepository.UpdateAsync(court);
        return MapToDto(updatedCourt);
    }

    public async Task DeleteCourtAsync(int courtId, int userId)
    {
        var court = await _courtRepository.GetByIdAsync(courtId);
        if (court == null)
        {
            throw new InvalidOperationException("Court not found");
        }

        if (court.Venue.OwnerId != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only the venue owner or admin can delete courts");
            }
        }

        if (await _courtRepository.HasFutureBookingsAsync(courtId))
        {
            throw new InvalidOperationException("Cannot delete court with future bookings");
        }

        await _courtRepository.DeleteAsync(courtId);
    }

    public async Task<IEnumerable<CourtResponseDto>> GetCourtsByVenueAsync(int venueId)
    {
        var courts = await _courtRepository.GetByVenueIdAsync(venueId);
        return courts.Select(MapToDto);
    }

    public async Task<CourtResponseDto?> GetCourtByIdAsync(int courtId)
    {
        var court = await _courtRepository.GetByIdAsync(courtId);
        return court == null ? null : MapToDto(court);
    }

    private CourtResponseDto MapToDto(Court court)
    {
        return new CourtResponseDto
        {
            CourtId = court.CourtId,
            VenueId = court.VenueId,
            VenueName = court.Venue?.Name ?? "",
            Name = court.Name,
            SportType = court.SportType,
            SlotDurationMinutes = court.SlotDurationMinutes,
            BasePrice = court.BasePrice,
            OperatingHours = court.OperatingHours,
            IsActive = court.IsActive,
            Rating = court.Rating,
            TotalRatings = court.TotalRatings
        };
    }
}
