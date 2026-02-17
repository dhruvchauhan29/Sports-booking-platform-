using SportsBookingPlatform.DTOs;

namespace SportsBookingPlatform.Services;

public interface ICourtService
{
    Task<CourtResponseDto> CreateCourtAsync(CreateCourtDto createCourtDto, int userId);
    Task<CourtResponseDto> UpdateCourtAsync(int courtId, UpdateCourtDto updateCourtDto, int userId);
    Task DeleteCourtAsync(int courtId, int userId);
    Task<IEnumerable<CourtResponseDto>> GetCourtsByVenueAsync(int venueId);
    Task<CourtResponseDto?> GetCourtByIdAsync(int courtId);
}
