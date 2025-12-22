using BarnManagement.Business.Workers;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using BarnManagement.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BarnManagement.Tests.Workers
{
    public class AnimalLifecycleWorkerTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<ILogger<AnimalLifecycleWorker>> _loggerMock;

        public AnimalLifecycleWorkerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _loggerMock = new Mock<ILogger<AnimalLifecycleWorker>>();
            
            // Setup Service Scope
            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(_serviceScopeFactoryMock.Object);
            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        }

        [Fact]
        public async Task ProcessLifecycle_ShouldRemoveDeadAnimals()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            _serviceProviderMock.Setup(x => x.GetService(typeof(AppDbContext))).Returns(context);

            var now = DateTime.UtcNow;
            var ownerId = Guid.NewGuid();
            var farmId = Guid.NewGuid();

            var user = new User { Id = ownerId, Email = "lifecycle@test.com", Username = "lifecycle", PasswordHash = new byte[0] };
            var farm = new Farm { Id = farmId, Name = "Lifecycle Farm", OwnerId = ownerId };
            
            context.Users.Add(user);
            context.Farms.Add(farm);
            await context.SaveChangesAsync();
            
            // Case 1: Henüz yaşamalı (Doğum tarihi 500 saniye önce)
            var aliveCow = new Animal { 
                Id = Guid.NewGuid(), 
                FarmId = farmId,
                Species = "Cow", 
                Name = "Alive", 
                LifeSpanDays = 20, 
                BirthDate = now.AddSeconds(-500) 
            };

            // Case 2: Ölmeli (Doğum tarihi 601 saniye önce)
            var deadCow = new Animal { 
                Id = Guid.NewGuid(), 
                FarmId = farmId,
                Species = "Cow", 
                Name = "Dead", 
                LifeSpanDays = 20, 
                BirthDate = now.AddSeconds(-601) 
            };

            context.Animals.AddRange(aliveCow, deadCow);
            await context.SaveChangesAsync();

            // Act
            var worker = new TestableAnimalLifecycleWorker(_serviceProviderMock.Object, _loggerMock.Object);
            await worker.TriggerProcessLifecycle(CancellationToken.None);

            // Assert
            var remainingAnimals = await context.Animals.ToListAsync();
            
            remainingAnimals.Should().Contain(a => a.Id == aliveCow.Id);
            remainingAnimals.Should().NotContain(a => a.Id == deadCow.Id);
        }
    }

    // Protected methoda erismek icin wrapper
    public class TestableAnimalLifecycleWorker : AnimalLifecycleWorker
    {
        public TestableAnimalLifecycleWorker(IServiceProvider sp, ILogger<AnimalLifecycleWorker> logger) : base(sp, logger) { }

        public async Task TriggerProcessLifecycle(CancellationToken token)
        {
             var method = typeof(AnimalLifecycleWorker).GetMethod("ProcessLifecycle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
             if(method != null)
             {
                 var task = (Task?)method.Invoke(this, new object[] { token });
                 if (task != null) await task;
             }
        }
    }
}
