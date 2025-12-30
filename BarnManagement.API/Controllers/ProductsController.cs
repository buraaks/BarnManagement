using System.Security.Claims;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

// Ürün işlemleri için JWT tabanlı yetkilendirme gereklidir.
[Authorize]
[ApiController]
[Route("api")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedAccessException();
        }
        return userId;
    }

    // Çiftlik stoğundaki ürünleri listele
    [HttpGet("farms/{farmId}/products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetFarmProducts(Guid farmId)
    {
        try
        {
            var userId = GetUserId();
            var products = await _productService.GetFarmProductsAsync(farmId, userId);
            return Ok(products);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // Belirtilen miktarda ürün satışı yap
    [HttpPost("products/{id}/sell")]
    public async Task<ActionResult<ProductDto>> SellProduct(Guid id, [FromQuery] int quantity = 1)
    {
        try
        {
            var userId = GetUserId();
            var product = await _productService.SellProductAsync(id, quantity, userId);
            
            if (product == null) return NotFound("Ürün bulunamadı.");
            return Ok(product);
        }
        catch (UnauthorizedAccessException)
        {
            // Kullanıcı bu ürünün sahibi olan çiftliğin sahibi değilse
            return Unauthorized("Bu ürünü satma yetkiniz yok.");
        }
        catch (InvalidOperationException ex)
        {
            // Stokta yeterli ürün yoksa gibi durumlar
            return BadRequest(ex.Message);
        }
    }

    // Ürün detaylarını getir
    [HttpGet("products/{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound("Ürün bulunamadı.");
        return Ok(product);
    }
    // Tüm ürünleri sat
    [HttpPost("farms/{farmId}/products/sell-all")]
    public async Task<ActionResult> SellAllProducts(Guid farmId)
    {
        try
        {
            var userId = GetUserId();
            var totalEarnings = await _productService.SellAllProductsAsync(farmId, userId);
            return Ok(new { totalEarnings });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Bu çiftliğin sahibi değilsiniz.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
