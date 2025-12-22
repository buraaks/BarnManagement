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

    public async Task<ProductDto?> SellProductAsync(Guid productId, Guid userId)
    {
        // 1. Ürünü bul
        var product = await _context.Products
            .Include(p => p.Farm)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found for sale", productId);
            return null;
        }



        // 3. Ürün sahibi kontrolü (Product → Farm → User)
        if (product.Farm == null) 
        {
             // Try to load farm if not included
             product.Farm = await _context.Farms.FindAsync(product.FarmId) 
                            ?? throw new InvalidOperationException("Farm associated with product not found.");
        }

        if (product.Farm.OwnerId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to sell product {ProductId} owned by {OwnerId}", 
                userId, productId, product.Farm.OwnerId);
            throw new UnauthorizedAccessException("You do not own this product.");
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
            user.Balance += product.SalePrice * product.Quantity;
            _context.Users.Update(user);

            // Ürünü sil (satıldı)
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("User {UserId} sold product {ProductId} ({ProductType}) for {Price}", 
                userId, productId, product.ProductType, product.SalePrice);

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

        // Farm'daki hayvanların ürünlerini getir
        var products = await _context.Products
            //.Include(p => p.Animal) // Animal is optional now
            .Where(p => p.FarmId == farmId)
            .ToListAsync();

        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return null;

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
        // 1. Çiftlik sahibi kontrolü
        var farm = await _context.Farms.FindAsync(farmId);
        if (farm == null || farm.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You do not own this farm.");
        }

        // 2. Satılacak ürünleri çek
        var products = await _context.Products
            .Where(p => p.FarmId == farmId) // Zaten sadece satılmamışlar duruyor
            .ToListAsync();

        if (!products.Any()) return 0m;

        decimal totalEarnings = products.Sum(p => p.SalePrice * p.Quantity);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 3. Kullanıcı bakiyesini güncelle
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Balance += totalEarnings;
                _context.Users.Update(user);
            }

            // 4. Ürünleri sil (Satış = Silme)
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
