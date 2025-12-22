using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarnManagement.Business.Services;

public class AnimalService : IAnimalService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AnimalService> _logger;

    public AnimalService(AppDbContext context, ILogger<AnimalService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AnimalDto> BuyAnimalAsync(Guid farmId, BuyAnimalRequest request, Guid userId)
    {
        // 1. Farm'ın var olduğunu ve kullanıcıya ait olduğunu kontrol et
        var farm = await _context.Farms.FindAsync(farmId);
        if (farm == null)
        {
            _logger.LogWarning("Farm {FarmId} not found for user {UserId}", farmId, userId);
            throw new InvalidOperationException("Farm not found.");
        }

        if (farm.OwnerId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to buy animal for farm {FarmId} owned by {OwnerId}", userId, farmId, farm.OwnerId);
            throw new UnauthorizedAccessException("You do not own this farm.");
        }

        // 2. Kullanıcının bakiyesini kontrol et
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (user.Balance < request.PurchasePrice)
        {
            _logger.LogWarning("User {UserId} has insufficient balance ({Balance}) for purchase ({Price})", userId, user.Balance, request.PurchasePrice);
            throw new InvalidOperationException("Insufficient balance.");
        }

        // 3. Transaction ile işlem yap
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Bakiyeyi düş
            user.Balance -= request.PurchasePrice;
            _context.Users.Update(user);

            // Satış fiyatını hesapla (satın alma fiyatının %80'i)
            var sellPrice = request.PurchasePrice * 0.8m;

            int lifeSpanYears = request.Species.ToLower() switch
            {
                "cow" or "inek" => 20,
                "sheep" or "koyun" => 15,
                "chicken" or "tavuk" => 10,
                _ => 10 // Varsayılan
            };

            // Animal kaydı oluştur
            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                FarmId = farmId,
                Species = request.Species,
                Name = request.Name,
                BirthDate = DateTime.UtcNow,
                LifeSpanDays = lifeSpanYears,
                ProductionInterval = request.ProductionInterval,
                NextProductionAt = DateTime.UtcNow.AddSeconds(request.ProductionInterval),
                PurchasePrice = request.PurchasePrice,
                SellPrice = sellPrice
            };

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("User {UserId} bought animal {AnimalId} ({Species}) for {Price}", userId, animal.Id, request.Species, request.PurchasePrice);

            return MapToDto(animal);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error buying animal for user {UserId}", userId);
            throw;
        }
    }

    public async Task<AnimalDto?> SellAnimalAsync(Guid animalId, Guid userId)
    {
        // 1. Hayvanı bul
        var animal = await _context.Animals
            .Include(a => a.Farm)
            .FirstOrDefaultAsync(a => a.Id == animalId);

        if (animal == null)
        {
            _logger.LogWarning("Animal {AnimalId} not found for sale", animalId);
            return null;
        }

        // 3. Farm sahibi kontrolü
        if (animal.Farm.OwnerId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to sell animal {AnimalId} owned by {OwnerId}", userId, animalId, animal.Farm.OwnerId);
            throw new UnauthorizedAccessException("You do not own this animal.");
        }

        // 4. Kullanıcıyı bul
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        // 5. Transaction ile işlem yap
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Bakiyeye satış fiyatını ekle
            user.Balance += animal.SellPrice;
            _context.Users.Update(user);

            // Hayvanı sil (Satış = Silme)
            _context.Animals.Remove(animal);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("User {UserId} sold animal {AnimalId} ({Species}) for {Price}", userId, animalId, animal.Species, animal.SellPrice);

            return MapToDto(animal);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error selling animal {AnimalId}", animalId);
            throw;
        }
    }

    public async Task<IEnumerable<AnimalDto>> GetFarmAnimalsAsync(Guid farmId, Guid userId)
    {
        // Farm'ın kullanıcıya ait olduğunu kontrol et
        var farm = await _context.Farms.FindAsync(farmId);
        if (farm == null || farm.OwnerId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to access animals for farm {FarmId}", userId, farmId);
            return Enumerable.Empty<AnimalDto>();
        }

        var animals = await _context.Animals
            .Where(a => a.FarmId == farmId)
            .ToListAsync();

        return animals.Select(MapToDto);
    }

    public async Task<AnimalDto?> GetAnimalByIdAsync(Guid animalId)
    {
        var animal = await _context.Animals.FindAsync(animalId);
        if (animal == null) return null;

        return MapToDto(animal);
    }

    private static AnimalDto MapToDto(Animal animal)
    {
        return new AnimalDto(
            animal.Id,
            animal.FarmId,
            animal.Species,
            animal.Name,
            animal.BirthDate,
            animal.LifeSpanDays,
            animal.ProductionInterval,
            animal.NextProductionAt,
            animal.PurchasePrice,
            animal.SellPrice
        );
    }
}
