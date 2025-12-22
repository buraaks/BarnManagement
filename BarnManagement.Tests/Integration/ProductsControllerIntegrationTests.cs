using BarnManagement.Core.DTOs;
using BarnManagement.Tests.Integration;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using BarnManagement.DataAccess.Entities;
using Microsoft.Extensions.DependencyInjection;
using BarnManagement.Core.Entities;

namespace BarnManagement.Tests.Integration
{
    public class ProductsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ProductsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _factory.InitializeDatabase();
        }

        private async Task<(HttpClient client, Guid farmId)> GetAuthenticatedClientAsync()
        {
            var client = _factory.CreateClient();
            var email = $"prod_{Guid.NewGuid()}@test.com";
            
            // Register
            var regReq = new RegisterRequest(email, "ProdUser", "Pass123!");
            var regRes = await client.PostAsJsonAsync("/api/auth/register", regReq);
            regRes.EnsureSuccessStatusCode();

            // Login
            var loginReq = new LoginRequest(email, "Pass123!");
            var logRes = await client.PostAsJsonAsync("/api/auth/login", loginReq);
            var authRes = await logRes.Content.ReadFromJsonAsync<AuthResponse>();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authRes!.Token);

            // Get default farm
            var farmsRes = await client.GetAsync("/api/farms");
            var farms = await farmsRes.Content.ReadFromJsonAsync<List<FarmDto>>();
            
            return (client, farms![0].Id);
        }

        [Fact]
        public async Task SellProduct_ShouldSucceed()
        {
            // Arrange
            var (client, farmId) = await GetAuthenticatedClientAsync();
            
            // Ürün ekle (Manuel DB)
            Guid productId = Guid.NewGuid();
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var product = new Product
                {
                    Id = productId,
                    FarmId = farmId,
                    ProductType = "Milk",
                    Quantity = 5,
                    SalePrice = 10,
                    ProducedAt = DateTime.UtcNow
                };
                context.Products.Add(product);
                await context.SaveChangesAsync();
            }

            // Act
            var response = await client.PostAsync($"/api/products/{productId}/sell", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ProductDto>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task SellAllProducts_ShouldSucceed()
        {
            // Arrange
            var (client, farmId) = await GetAuthenticatedClientAsync();

            // Ürünler ekle
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Products.Add(new Product { Id = Guid.NewGuid(), FarmId = farmId, ProductType = "Milk", Quantity = 2, SalePrice = 10, ProducedAt = DateTime.UtcNow });
                context.Products.Add(new Product { Id = Guid.NewGuid(), FarmId = farmId, ProductType = "Egg", Quantity = 5, SalePrice = 2, ProducedAt = DateTime.UtcNow });
                await context.SaveChangesAsync();
            }

            // Act
            var response = await client.PostAsync($"/api/farms/{farmId}/products/sell-all", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("30"); // (2*10) + (5*2) = 30
        }
    }
}
