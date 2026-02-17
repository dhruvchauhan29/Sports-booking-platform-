using SportsBookingPlatform.DTOs;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.Services;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly ICourtRepository _courtRepository;
    private readonly IUserRepository _userRepository;

    public DiscountService(
        IDiscountRepository discountRepository,
        IVenueRepository venueRepository,
        ICourtRepository courtRepository,
        IUserRepository userRepository)
    {
        _discountRepository = discountRepository;
        _venueRepository = venueRepository;
        _courtRepository = courtRepository;
        _userRepository = userRepository;
    }

    public async Task<DiscountResponseDto> CreateDiscountAsync(CreateDiscountDto createDiscountDto, int userId)
    {
        // Validate dates
        if (createDiscountDto.ValidFrom >= createDiscountDto.ValidTo)
        {
            throw new InvalidOperationException("ValidFrom must be before ValidTo");
        }

        // Validate scope and IDs
        if (createDiscountDto.Scope == DiscountScope.Venue)
        {
            if (!createDiscountDto.VenueId.HasValue)
            {
                throw new InvalidOperationException("VenueId is required for venue-scoped discounts");
            }

            var venue = await _venueRepository.GetByIdAsync(createDiscountDto.VenueId.Value);
            if (venue == null)
            {
                throw new InvalidOperationException("Venue not found");
            }

            // Check ownership
            if (venue.OwnerId != userId)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.Role != UserRole.Admin)
                {
                    throw new UnauthorizedAccessException("Only the venue owner or admin can create discounts");
                }
            }
        }
        else if (createDiscountDto.Scope == DiscountScope.Court)
        {
            if (!createDiscountDto.CourtId.HasValue)
            {
                throw new InvalidOperationException("CourtId is required for court-scoped discounts");
            }

            var court = await _courtRepository.GetByIdAsync(createDiscountDto.CourtId.Value);
            if (court == null)
            {
                throw new InvalidOperationException("Court not found");
            }

            // Check ownership
            if (court.Venue.OwnerId != userId)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.Role != UserRole.Admin)
                {
                    throw new UnauthorizedAccessException("Only the venue owner or admin can create discounts");
                }
            }
        }

        var discount = new Discount
        {
            Scope = createDiscountDto.Scope,
            VenueId = createDiscountDto.VenueId,
            CourtId = createDiscountDto.CourtId,
            PercentOff = createDiscountDto.PercentOff,
            ValidFrom = createDiscountDto.ValidFrom,
            ValidTo = createDiscountDto.ValidTo,
            IsActive = true
        };

        var createdDiscount = await _discountRepository.CreateAsync(discount);
        return MapToDto(createdDiscount);
    }

    public async Task<IEnumerable<DiscountResponseDto>> GetAllDiscountsAsync()
    {
        var discounts = await _discountRepository.GetAllAsync();
        return discounts.Select(MapToDto);
    }

    public async Task<IEnumerable<DiscountResponseDto>> GetActiveDiscountsAsync()
    {
        var discounts = await _discountRepository.GetActiveDiscountsAsync();
        return discounts.Select(MapToDto);
    }

    public async Task<DiscountResponseDto?> GetDiscountByIdAsync(int discountId)
    {
        var discount = await _discountRepository.GetByIdAsync(discountId);
        return discount == null ? null : MapToDto(discount);
    }

    private DiscountResponseDto MapToDto(Discount discount)
    {
        return new DiscountResponseDto
        {
            DiscountId = discount.DiscountId,
            Scope = discount.Scope.ToString(),
            VenueId = discount.VenueId,
            VenueName = discount.Venue?.Name,
            CourtId = discount.CourtId,
            CourtName = discount.Court?.Name,
            PercentOff = discount.PercentOff,
            ValidFrom = discount.ValidFrom,
            ValidTo = discount.ValidTo,
            IsActive = discount.IsActive
        };
    }
}
