using BarnManagement.Business.Services;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.Core.Enums;
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
    public class AnimalServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<ILogger<AnimalService>> _loggerMock;

        public AnimalServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _loggerMock = new Mock<ILogger<AnimalService>>();
        }

        [Fact]
        public async Task BuyAnimalAsync_ShouldSuccess_WhenBalanceIsSufficient()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var animalService = new AnimalService(context, _loggerMock.Object);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "buyer@test.com", Username = "buyer", Balance = 2000, PasswordHash = new byte[0] };
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Buyer's Farm", OwnerId = userId };
            
            context.Users.Add(user);
            context.Farms.Add(farm);
            await context.SaveChangesAsync();

            var request = new BuyAnimalRequest(AnimalSpecies.Cow, "Daisy", 500, 60);

            // Act
            var result = await animalService.BuyAnimalAsync(farm.Id, request, userId);

            // Assert
            result.Should().NotBeNull();
            result.Species.Should().Be(AnimalSpecies.Cow);
            result.PurchasePrice.Should().Be(500);

            var updatedUser = await context.Users.FindAsync(userId);
            updatedUser.Should().NotBeNull();
            updatedUser!.Balance.Should().Be(1500); // 2000 - 500

            var animal = await context.Animals.FindAsync(result.Id);
            animal.Should().NotBeNull();
            animal.FarmId.Should().Be(farm.Id);
        }

        [Fact]
        public async Task BuyAnimalAsync_ShouldThrow_WhenBalanceIsInsufficient()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var animalService = new AnimalService(context, _loggerMock.Object);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "poor@test.com", Username = "poor", Balance = 100, PasswordHash = new byte[0] };
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Poor Farm", OwnerId = userId };

            context.Users.Add(user);
            context.Farms.Add(farm);
            await context.SaveChangesAsync();

            var request = new BuyAnimalRequest(AnimalSpecies.Cow, "TooExpensive", 500, 60);

            // Act
            Func<Task> act = async () => await animalService.BuyAnimalAsync(farm.Id, request, userId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Insufficient balance.");
        }

        [Fact]
        public async Task SellAnimalAsync_ShouldSuccess_AndIncreaseBalance()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var animalService = new AnimalService(context, _loggerMock.Object);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "seller@test.com", Username = "seller", Balance = 100, PasswordHash = new byte[0] };
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Seller's Farm", OwnerId = userId };
            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                FarmId = farm.Id,
                Species = AnimalSpecies.Sheep,
                Name = "ToSell",
                SellPrice = 300,
                Farm = farm
            };

            context.Users.Add(user);
            context.Farms.Add(farm);
            context.Animals.Add(animal);
            await context.SaveChangesAsync();

            // Act
            var result = await animalService.SellAnimalAsync(animal.Id, userId);

            // Assert
            result.Should().NotBeNull();
            
            var updatedUser = await context.Users.FindAsync(userId);
            updatedUser.Should().NotBeNull();
            updatedUser!.Balance.Should().Be(400); // 100 + 300

            var deletedAnimal = await context.Animals.FindAsync(animal.Id);
            deletedAnimal.Should().BeNull();
        }

        [Fact]
        public async Task SellAnimalAsync_ShouldThrow_WhenUserDoesNotOwnFarm()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var animalService = new AnimalService(context, _loggerMock.Object);

            var ownerId = Guid.NewGuid();
            var thiefId = Guid.NewGuid();
            var user = new User { Id = ownerId, Email = "owner@test.com", Username = "owner", PasswordHash = new byte[0] };
            context.Users.Add(user);
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Owner's Farm", OwnerId = ownerId };
            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                FarmId = farm.Id,
                Species = AnimalSpecies.Cow,
                Name = "Owner's Cow",
                Farm = farm
            };

            context.Farms.Add(farm);
            context.Animals.Add(animal);
            await context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await animalService.SellAnimalAsync(animal.Id, thiefId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GetAnimalByIdAsync_ShouldReturnNull_IfNotOwnedByUser()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var animalService = new AnimalService(context, _loggerMock.Object);

            var ownerId = Guid.NewGuid();
            var thiefId = Guid.NewGuid();
            var user = new User { Id = ownerId, Email = "animal_owner@test.com", Username = "animalowner", PasswordHash = new byte[0] };
            context.Users.Add(user);
            
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Secure Farm", OwnerId = ownerId };
            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                FarmId = farm.Id,
                Species = AnimalSpecies.Cow,
                Name = "Secret Cow",
                Farm = farm
            };

            context.Farms.Add(farm);
            context.Animals.Add(animal);
            await context.SaveChangesAsync();

            // Act
            var result = await animalService.GetAnimalByIdAsync(animal.Id, thiefId);

            // Assert
            result.Should().BeNull();
        }
    }
}
