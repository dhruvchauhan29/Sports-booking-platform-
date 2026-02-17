using SportsBookingPlatform.Enums;
using SportsBookingPlatform.Repositories;

namespace SportsBookingPlatform.BackgroundServices;

public class SlotLockExpiryService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SlotLockExpiryService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public SlotLockExpiryService(
        IServiceProvider serviceProvider,
        ILogger<SlotLockExpiryService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Slot Lock Expiry Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredLocksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expired slot locks");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Slot Lock Expiry Service is stopping");
    }

    private async Task ProcessExpiredLocksAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var slotRepository = scope.ServiceProvider.GetRequiredService<ISlotRepository>();

        var expiredSlots = await slotRepository.GetExpiredLockedSlotsAsync();
        
        foreach (var slot in expiredSlots)
        {
            slot.Status = BookingStatus.Available;
            slot.LockToken = null;
            slot.LockedUntil = null;
            
            await slotRepository.UpdateAsync(slot);
            
            _logger.LogInformation("Released expired lock for slot {SlotId}", slot.SlotId);
        }
    }
}
