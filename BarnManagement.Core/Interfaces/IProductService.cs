using BarnManagement.Core.DTOs;

namespace BarnManagement.Core.Interfaces;
// Kısa mimari özet: Bu arayüz, ürün süreçlerine ait servis operasyonlarının sözleşmesini tanımlar.


public interface IProductService
{
    Task<ProductDto?> SellProductAsync(Guid productId, int quantity, Guid userId);
    Task<IEnumerable<ProductDto>> GetFarmProductsAsync(Guid farmId, Guid userId);
    Task<ProductDto?> GetProductByIdAsync(Guid productId, Guid userId);
    Task<decimal> SellAllProductsAsync(Guid farmId, Guid userId);
}
