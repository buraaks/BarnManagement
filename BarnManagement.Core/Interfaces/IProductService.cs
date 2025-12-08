using BarnManagement.Core.DTOs;

namespace BarnManagement.Core.Interfaces;

public interface IProductService
{
    Task<ProductDto?> SellProductAsync(Guid productId, Guid userId);
    Task<IEnumerable<ProductDto>> GetFarmProductsAsync(Guid farmId, Guid userId);
    Task<ProductDto?> GetProductByIdAsync(Guid productId);
}
