using BarnManagement.Business.Services;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using BarnManagement.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BarnManagement.Tests.Services
{
    public class FarmServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public FarmServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateFarmAsync_ShouldCreate_AndReturnDto()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var farmService = new FarmService(context);

            var ownerId = Guid.NewGuid();
            var user = new User { Id = ownerId, Email = "farmowner@test.com", Username = "owner", PasswordHash = new byte[0] };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var request = new CreateFarmRequest("New Farm");

            // Act
            var result = await farmService.CreateFarmAsync(request, ownerId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Farm");
            result.OwnerId.Should().Be(ownerId);

            var farm = await context.Farms.FindAsync(result.Id);
            farm.Should().NotBeNull();
            farm.Name.Should().Be("New Farm");
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldUpdate_IfOwnedByUser()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var farmService = new FarmService(context);

            var ownerId = Guid.NewGuid();
            var user = new User { Id = ownerId, Email = "update@test.com", Username = "update", PasswordHash = new byte[0] };
            context.Users.Add(user);
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Old Name", OwnerId = ownerId };
            context.Farms.Add(farm);
            await context.SaveChangesAsync();

            var request = new UpdateFarmRequest("Updated Name");

            // Act
            var result = await farmService.UpdateFarmAsync(farm.Id, request, ownerId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Name");

            var updatedFarm = await context.Farms.FindAsync(farm.Id);
            updatedFarm.Should().NotBeNull();
            updatedFarm!.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnTrue_IfDeleted()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var farmService = new FarmService(context);

            var ownerId = Guid.NewGuid();
            var user = new User { Id = ownerId, Email = "delete@test.com", Username = "delete", PasswordHash = new byte[0] };
            context.Users.Add(user);
            var farm = new Farm { Id = Guid.NewGuid(), Name = "To Delete", OwnerId = ownerId };
            context.Farms.Add(farm);
            await context.SaveChangesAsync();

            // Act
            var result = await farmService.DeleteFarmAsync(farm.Id, ownerId);

            // Assert
            result.Should().BeTrue();
            var deletedFarm = await context.Farms.FindAsync(farm.Id);
            deletedFarm.Should().BeNull();
        }

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnNull_IfNotOwnedByUser()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var farmService = new FarmService(context);

            var ownerId = Guid.NewGuid();
            var thiefId = Guid.NewGuid();
            var user = new User { Id = ownerId, Email = "owner_secure@test.com", Username = "owner_secure", PasswordHash = new byte[0] };
            context.Users.Add(user);
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Secure Farm", OwnerId = ownerId };
            context.Farms.Add(farm);
            await context.SaveChangesAsync();

            // Act
            var result = await farmService.GetFarmByIdAsync(farm.Id, thiefId);

            // Assert
            result.Should().BeNull();
        }
    }
}
