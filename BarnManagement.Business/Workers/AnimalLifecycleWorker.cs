using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BarnManagement.Business.Workers;

public class AnimalLifecycleWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AnimalLifecycleWorker> _logger;
    // Test için 10 saniye yapalım (Hızlı Simülasyon)
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10); 

    public AnimalLifecycleWorker(IServiceProvider serviceProvider, ILogger<AnimalLifecycleWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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

        // Ömrü dolan hayvanları bul
        // SİMÜLASYON MODU: LifeSpanDays = Dakika cinsinden ömür
        // Yani 5 girildiyse 5 dakika sonra ölür.
        var deadAnimals = await context.Animals
            .ToListAsync(stoppingToken); // Tümünü çekip bellekte filtrelemek zorunda kalabiliriz çünkü EF Core DateTime farklarını tam çeviremeyebilir, veya SQL DATEDIFF kullanılabilir.
            
        // Bellekte filtreleme (Daha güvenli SQL çevirimi için)
        // LifeSpanDays artık saniye olarak tutuluyor.
        var animalsToRemove = deadAnimals
            .Where(a => (now - a.BirthDate).TotalSeconds >= a.LifeSpanDays)
            .ToList();

        if (animalsToRemove.Any())
        {
            _logger.LogInformation("Found {Count} animals that reached end of life.", animalsToRemove.Count);

            foreach (var animal in animalsToRemove)
            {
                context.Animals.Remove(animal);
                // Log formatted for simulation years (1 Year = 30 Seconds)
                var ageYears = (now - animal.BirthDate).TotalSeconds / 30.0;
                _logger.LogInformation("Animal {AnimalId} ({Species}) has died (Age: {Age:F1} Years) and removed.", animal.Id, animal.Species, ageYears);
            }

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
