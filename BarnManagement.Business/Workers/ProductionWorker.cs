using BarnManagement.Core.Entities;
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
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2); // Hızlı Simülasyon için 2 saniyede bir kontrol

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

        // Üretim zamanı gelmiş (veya geçmiş) hayvanları bul
        var readyAnimals = await context.Animals
            .Where(a => a.NextProductionAt <= now)
            .ToListAsync(stoppingToken);

        if (readyAnimals.Any())
        {
            _logger.LogInformation("Found {Count} animals ready to produce.", readyAnimals.Count);

            foreach (var animal in readyAnimals)
            {
                // Transaction her hayvan için ayrı olabilir veya toplu yapılabilir. 
                // Hata toleransı için tek tek yapalım.
                using var transaction = await context.Database.BeginTransactionAsync(stoppingToken);
                try
                {
                    // 1. Ürün oluştur
                    // 1. Ürün oluştur veya miktar artır
                    var productType = GetProductType(animal.Species);
                    
                    var existingProduct = await context.Products
                        .FirstOrDefaultAsync(p => p.AnimalId == animal.Id && p.ProductType == productType, stoppingToken);

                    if (existingProduct != null)
                    {
                        existingProduct.Quantity += 1;
                        existingProduct.ProducedAt = now; // Son üretim zamanını guncelle
                        context.Products.Update(existingProduct);
                        _logger.LogInformation("Animal {AnimalId} produced {ProductType} (Total Quantity: {Quantity}).", animal.Id, productType, existingProduct.Quantity);
                    }
                    else
                    {
                        var product = new Product
                        {
                            AnimalId = animal.Id,
                            ProductType = productType,
                            ProducedAt = now,
                            SalePrice = CalculateSalePrice(animal.Species),
                            Quantity = 1
                        };
                        context.Products.Add(product);
                         _logger.LogInformation("Animal {AnimalId} produced {ProductType} (New Product).", animal.Id, productType);
                    }

                    // 2. Bir sonraki üretim zamanını güncelle
                    // Eğer çok gecikmişse, şimdiki zamandan değil, olması gereken zamandan ekleyerek gitmek daha doğru olabilir 
                    // ama basitleştirmek için "Now + Interval" kullanabiliriz.
                    // Ya da: Math.Max(Now, NextProductionAt + Interval)
                    animal.NextProductionAt = now.AddSeconds(animal.ProductionInterval);
                    context.Animals.Update(animal);

                    await context.SaveChangesAsync(stoppingToken);
                    await transaction.CommitAsync(stoppingToken);


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process production for Animal {AnimalId}", animal.Id);
                    await transaction.RollbackAsync(stoppingToken);
                }
            }
        }
    }

    private string GetProductType(string species)
    {
        return species.ToLower() switch
        {
            "cow" or "inek" => "Milk",
            "chicken" or "tavuk" => "Egg",
            "sheep" or "koyun" => "Wool",
            _ => "Unknown"
        };
    }

    private decimal CalculateSalePrice(string species)
    {
        //  fiyatlandırma
        return species.ToLower() switch
        {
            "cow" or "inek" => 15.0m,    // Süt
            "chicken" or "tavuk" => 2.5m, // Yumurta
            "sheep" or "koyun" => 50.0m,  // Yün
            _ => 1.0m
        };
    }
}
