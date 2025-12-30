using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarnManagement.Business.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductDto?> SellProductAsync(Guid productId, int quantity, Guid userId)
    {
        var product = await _context.Products
            .Include(p => p.Farm)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found for sale", productId);
            return null;
        }

        // Ürün sahibi kontrolü (Product -> Farm -> User)
        if (product.Farm == null) 
        {
             // Eğer Farm henüz yüklenmemişse, veritabanından getir
             product.Farm = await _context.Farms.FindAsync(product.FarmId) 
                            ?? throw new InvalidOperationException("Farm associated with product not found.");
        }

        if (product.Farm.OwnerId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to sell product {ProductId} owned by {OwnerId}", 
                userId, productId, product.Farm.OwnerId);
            throw new UnauthorizedAccessException("You do not own this product.");
        }

        if (product.Quantity < quantity)
        {
            throw new InvalidOperationException("Stokta yeterli ürün yok.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        // Satışı atomik bir işlem olarak gerçekleştir
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Bakiyeye satış fiyatını (miktar x birim fiyat) ekle
            user.Balance += product.SalePrice * quantity;
            _context.Users.Update(user);

            // Miktarı düşür veya ürünü sil
            product.Quantity -= quantity;
            if (product.Quantity <= 0)
            {
                _context.Products.Remove(product);
            }
            else
            {
                _context.Products.Update(product);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("User {UserId} sold {Quantity} units of product {ProductId} ({ProductType}) for {Price} each", 
                userId, quantity, productId, product.ProductType, product.SalePrice);

            return MapToDto(product);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error selling product {ProductId}", productId);
            throw;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetFarmProductsAsync(Guid farmId, Guid userId)
    {
        // Farm'ın kullanıcıya ait olduğunu kontrol et
        var farm = await _context.Farms.FindAsync(farmId);
        if (farm == null || farm.OwnerId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to access products for farm {FarmId}", userId, farmId);
            return Enumerable.Empty<ProductDto>();
        }

        // Çiftlikteki mevcut ürünleri getir
        var products = await _context.Products
            .Where(p => p.FarmId == farmId)
            .ToListAsync();

        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid productId, Guid userId)
    {
        var product = await _context.Products
            .Include(p => p.Farm)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null || product.Farm?.OwnerId != userId)
        {
            return null;
        }

        return MapToDto(product);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.ProductType,
            product.SalePrice,
            product.ProducedAt,
            product.Quantity
        );
    }

    public async Task<decimal> SellAllProductsAsync(Guid farmId, Guid userId)
    {
        // Çiftlik sahibi kontrolü
        var farm = await _context.Farms.FindAsync(farmId);
        if (farm == null || farm.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You do not own this farm.");
        }

        var products = await _context.Products
            .Where(p => p.FarmId == farmId)
            .ToListAsync();

        if (!products.Any()) return 0m;

        decimal totalEarnings = products.Sum(p => p.SalePrice * p.Quantity);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Kullanıcı bakiyesini toplam kazanç kadar artır
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Balance += totalEarnings;
                _context.Users.Update(user);
            }

            // Satılan ürünleri sistemden temizle
            _context.Products.RemoveRange(products);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("User {UserId} sold all {Count} products for {Total}", userId, products.Count, totalEarnings);

            return totalEarnings;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error selling all products for farm {FarmId}", farmId);
            throw;
        }
    }
}
