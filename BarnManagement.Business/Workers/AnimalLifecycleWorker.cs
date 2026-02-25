using BarnManagement.Core.Interfaces;
using BarnManagement.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BarnManagement.Business.Workers;

public class AnimalLifecycleWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AnimalLifecycleWorker> _logger;
    private readonly ISseBroadcaster _broadcaster;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);

    public AnimalLifecycleWorker(IServiceProvider serviceProvider, ILogger<AnimalLifecycleWorker> logger, ISseBroadcaster broadcaster)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _broadcaster = broadcaster;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Lifecycle Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessLifecycle(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing Lifecycle Worker.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Lifecycle Worker is stopping.");
    }

    private async Task ProcessLifecycle(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        var deadAnimals = await context.Animals.ToListAsync(stoppingToken);

        var animalsToRemove = deadAnimals
            .Where(a => {
                var lifeSpanSeconds = a.LifeSpanDays > 100 ? a.LifeSpanDays : (a.LifeSpanDays * 30);
                var ageSeconds = (now - a.BirthDate).TotalSeconds;
                var isDead = ageSeconds >= lifeSpanSeconds;

                return isDead;
            })
            .ToList();

        if (animalsToRemove.Any())
        {
            _logger.LogInformation("Found {Count} animals that reached end of life.", animalsToRemove.Count);

            foreach (var animal in animalsToRemove)
            {
                context.Animals.Remove(animal);
                var ageYears = (now - animal.BirthDate).TotalSeconds / 30.0;
                _logger.LogInformation("Animal {AnimalId} ({Species}) has died (Age: {Age:F1} Years) and removed.", animal.Id, animal.Species, ageYears);
            }

            await context.SaveChangesAsync(stoppingToken);
            await _broadcaster.BroadcastUpdateAsync("refresh", "lifecycle", stoppingToken);
        }
    }
}
