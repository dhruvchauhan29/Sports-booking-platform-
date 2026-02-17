using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Data;
using SportsBookingPlatform.Entities;
using SportsBookingPlatform.Enums;

namespace SportsBookingPlatform.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly ApplicationDbContext _context;

    public VenueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Venue?> GetByIdAsync(int venueId)
    {
        return await _context.Venues
            .Include(v => v.Owner)
            .Include(v => v.Courts)
            .FirstOrDefaultAsync(v => v.VenueId == venueId);
    }

    public async Task<IEnumerable<Venue>> GetAllAsync()
    {
        return await _context.Venues
            .Include(v => v.Owner)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venue>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Venues
            .Include(v => v.Owner)
            .Where(v => v.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venue>> GetByApprovalStatusAsync(ApprovalStatus status)
    {
        return await _context.Venues
            .Include(v => v.Owner)
            .Where(v => v.ApprovalStatus == status)
            .ToListAsync();
    }

    public async Task<Venue> CreateAsync(Venue venue)
    {
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync();
        return venue;
    }

    public async Task<Venue> UpdateAsync(Venue venue)
    {
        _context.Venues.Update(venue);
        await _context.SaveChangesAsync();
        return venue;
    }

    public async Task DeleteAsync(int venueId)
    {
        var venue = await _context.Venues.FindAsync(venueId);
        if (venue != null)
        {
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
        }
    }
}
