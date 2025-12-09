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
            .Include(p => p.Animal)
                .ThenInclude(a => a.Farm)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found for sale", productId);
            return null;
        }



        // 3. Ürün sahibi kontrolü (Animal → Farm → User)
        if (product.Animal.Farm.OwnerId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to sell product {ProductId} owned by {OwnerId}", 
                userId, productId, product.Animal.Farm.OwnerId);
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
            user.Balance += product.SalePrice;
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
            .Include(p => p.Animal)
            .Where(p => p.Animal.FarmId == farmId)
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
            product.AnimalId,
            product.ProductType,
            product.SalePrice,
            product.ProducedAt
        );
    }
}
