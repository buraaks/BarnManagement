using BarnManagement.Core.Interfaces;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using BarnManagement.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BarnManagement.DataAccess.Entities;
using BarnManagement.Core.Entities;
using BarnManagement.Core.Enums;
using BarnManagement.Business.Workers;

namespace BarnManagement.Tests.Workers
{
    public class ProductionWorkerTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<ILogger<ProductionWorker>> _loggerMock;
        private readonly Mock<IMarketService> _marketServiceMock;

        public ProductionWorkerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _loggerMock = new Mock<ILogger<ProductionWorker>>();
            _marketServiceMock = new Mock<IMarketService>();
 
            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(_serviceScopeFactoryMock.Object);
            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IMarketService))).Returns(_marketServiceMock.Object);
        }

        [Fact]
        public async Task ProcessProductions_ShouldProduceItems_And_UpdateNextProductionTime()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            _serviceProviderMock.Setup(x => x.GetService(typeof(AppDbContext))).Returns(context);
            _marketServiceMock.Setup(x => x.GetProductPrice("Egg")).Returns(2.5m);

            var now = DateTime.UtcNow;
            var ownerId = Guid.NewGuid();
            var farmId = Guid.NewGuid();

            var user = new User { Id = ownerId, Email = "worker@test.com", Username = "worker", PasswordHash = new byte[0] };
            var farm = new Farm { Id = farmId, Name = "Worker Farm", OwnerId = ownerId };
            
            context.Users.Add(user);
            context.Farms.Add(farm);
            await context.SaveChangesAsync();

            // Üretim zamanı gelmiş bir Tavuk (NextProductionAt < Now)
            var readyChicken = new Animal
            {
                Id = Guid.NewGuid(),
                FarmId = farmId,
                Species = AnimalSpecies.Chicken, // Produces Egg
                Name = "Henrietta",
                BirthDate = now.AddMonths(-1),
                NextProductionAt = now.AddMinutes(-5), // 5 dakika gecikmis
                ProductionInterval = 60 // 60 saniye
            };

            // Üretim zamanı gelmemiş bir İnek
            var waitingCow = new Animal
            {
                Id = Guid.NewGuid(),
                FarmId = farmId,
                Species = AnimalSpecies.Cow,
                Name = "Bessie",
                BirthDate = now.AddMonths(-1),
                NextProductionAt = now.AddMinutes(10),
                ProductionInterval = 300
            };

            context.Animals.AddRange(readyChicken, waitingCow);
            await context.SaveChangesAsync();

            // Act
            var worker = new TestableProductionWorker(_serviceProviderMock.Object, _loggerMock.Object);
            await worker.TriggerProcessProductions(CancellationToken.None);

            // Assert
            
            // 1. Ürün oluşmuş mu?
            var products = await context.Products.ToListAsync();
            products.Should().HaveCount(1);
            
            var egg = products[0];
            egg.ProductType.Should().Be("Egg"); // Tavuk yumurta üretir
            egg.FarmId.Should().Be(farmId);
            egg.Quantity.Should().Be(1);

            // 2. Tavuğun bir sonraki üretim zamanı güncellenmiş mi?
            var updatedChicken = await context.Animals.FindAsync(readyChicken.Id);
            updatedChicken.NextProductionAt.Should().BeAfter(now); // Geleceğe atılmış olmalı (Now + Interval)
            
            // 3. İnek işlem görmemeli
            var updatedCow = await context.Animals.FindAsync(waitingCow.Id);
            updatedCow.NextProductionAt.Should().Be(waitingCow.NextProductionAt); // Değişmemeli
        }
    }

    public class TestableProductionWorker : ProductionWorker
    {
        public TestableProductionWorker(IServiceProvider sp, ILogger<ProductionWorker> logger) : base(sp, logger) { }

        public async Task TriggerProcessProductions(CancellationToken token)
        {
             var method = typeof(ProductionWorker).GetMethod("ProcessProductions", BindingFlags.NonPublic | BindingFlags.Instance);
             if(method != null)
             {
                 var task = (Task?)method.Invoke(this, new object[] { token });
                 if (task != null) await task;
             }
        }
    }
}
