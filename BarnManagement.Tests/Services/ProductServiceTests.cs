using BarnManagement.Business.Services;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using BarnManagement.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BarnManagement.Tests.Services
{
    public class ProductServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<ILogger<ProductService>> _loggerMock;

        public ProductServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _loggerMock = new Mock<ILogger<ProductService>>();
        }

        [Fact]
        public async Task SellProductAsync_ShouldIncreaseBalance_AndRemoveProduct()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var productService = new ProductService(context, _loggerMock.Object);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "prod_seller@test.com", Username = "prodseller", Balance = 100, PasswordHash = new byte[0] };
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Prod Farm", OwnerId = userId };
            var product = new Product
            {
                Id = Guid.NewGuid(),
                FarmId = farm.Id,
                ProductType = "Egg",
                Quantity = 10,
                SalePrice = 5,
                Farm = farm,
                ProducedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.Farms.Add(farm);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.SellProductAsync(product.Id, userId);

            // Assert
            result.Should().NotBeNull();
            
            var updatedUser = await context.Users.FindAsync(userId);
            updatedUser.Balance.Should().Be(150); // 100 + (10 * 5)

            var deletedProduct = await context.Products.FindAsync(product.Id);
            deletedProduct.Should().BeNull();
        }

        [Fact]
        public async Task SellAllProductsAsync_ShouldSellEverything_OnFarm()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var productService = new ProductService(context, _loggerMock.Object);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "batch_seller@test.com", Username = "batch", Balance = 0, PasswordHash = new byte[0] };
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Batch Farm", OwnerId = userId };
            
            var p1 = new Product { Id = Guid.NewGuid(), FarmId = farm.Id, ProductType = "Milk", Quantity = 2, SalePrice = 50, Farm = farm, ProducedAt = DateTime.UtcNow };
            var p2 = new Product { Id = Guid.NewGuid(), FarmId = farm.Id, ProductType = "Egg", Quantity = 10, SalePrice = 2, Farm = farm, ProducedAt = DateTime.UtcNow };

            context.Users.Add(user);
            context.Farms.Add(farm);
            context.Products.AddRange(p1, p2);
            await context.SaveChangesAsync();

            // Act
            var totalEarnings = await productService.SellAllProductsAsync(farm.Id, userId);

            // Assert
            totalEarnings.Should().Be(120); // (2*50) + (10*2)
            
            var updatedUser = await context.Users.FindAsync(userId);
            updatedUser.Balance.Should().Be(120);

            var remainingProducts = context.Products.Where(p => p.FarmId == farm.Id);
            remainingProducts.Should().BeEmpty();
        }
    }
}
