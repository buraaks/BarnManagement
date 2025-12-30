using BarnManagement.Core.Entities;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Enums;
using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BarnManagement.Business.Workers;

public class ProductionWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProductionWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);

    public ProductionWorker(IServiceProvider serviceProvider, ILogger<ProductionWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Production Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessProductions(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing Production Worker.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Production Worker is stopping.");
    }

    private async Task ProcessProductions(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        var readyAnimals = await context.Animals
            .Where(a => a.NextProductionAt <= now)
            .ToListAsync(stoppingToken);

        if (readyAnimals.Any())
        {
            _logger.LogInformation("Found {Count} animals ready to produce.", readyAnimals.Count);

            foreach (var animal in readyAnimals)
            {
                try
                {
                    var productType = GetProductType(animal.Species);
                    var marketService = scope.ServiceProvider.GetRequiredService<IMarketService>();
                    var currentPrice = marketService.GetProductPrice(productType);
                    
                    var existingProduct = await context.Products
                        .FirstOrDefaultAsync(p => p.FarmId == animal.FarmId && p.ProductType == productType, stoppingToken);

                    if (existingProduct != null)
                    {
                        existingProduct.Quantity += 1;
                        existingProduct.ProducedAt = now;
                        context.Products.Update(existingProduct);
                        _logger.LogInformation("Animal {AnimalId} produced {ProductType} (Total Quantity: {Quantity}).", animal.Id, productType, existingProduct.Quantity);
                    }
                    else
                    {
                        var product = new Product
                        {
                            FarmId = animal.FarmId,
                            ProductType = productType,
                            ProducedAt = now,
                            SalePrice = currentPrice,
                            Quantity = 1
                        };
                        context.Products.Add(product);
                         _logger.LogInformation("Animal {AnimalId} produced {ProductType} (New Product).", animal.Id, productType);
                    }

                    animal.NextProductionAt = now.AddSeconds(animal.ProductionInterval);
                    context.Animals.Update(animal);

                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process production for Animal {AnimalId}", animal.Id);
                }
            }
        }
    }

    private string GetProductType(AnimalSpecies species)
    {
        return species switch
        {
            AnimalSpecies.Cow => "Milk",
            AnimalSpecies.Chicken => "Egg",
            AnimalSpecies.Sheep => "Wool",
            _ => "Unknown"
        };
    }
}
