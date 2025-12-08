using System.Security.Claims;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

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

    /// <summary>
    /// Ürün satışı
    /// </summary>
    [HttpPost("products/{id}/sell")]
    public async Task<ActionResult<ProductDto>> SellProduct(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var product = await _productService.SellProductAsync(id, userId);

            if (product == null) return NotFound("Product not found.");

            return Ok(product);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("You do not own this product.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Çiftliğin ürünlerini listele
    /// </summary>
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

    /// <summary>
    /// Ürün detayı
    /// </summary>
    [HttpGet("products/{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound("Product not found.");
        return Ok(product);
    }
}
