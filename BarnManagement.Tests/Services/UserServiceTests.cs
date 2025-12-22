using BarnManagement.Business.Services;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using BarnManagement.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BarnManagement.Tests.Services
{
    public class UserServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public UserServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var userService = new UserService(context);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "testuser@example.com", Username = "testuser", Balance = 500, PasswordHash = new byte[0] };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await userService.GetUserByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(user.Email);
            result.Username.Should().Be(user.Username);
        }

        [Fact]
        public async Task GetUserBalanceAsync_ShouldReturnCorrectBalance()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var userService = new UserService(context);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "balance@test.com", Username = "balanceuser", Balance = 1234.56m, PasswordHash = new byte[0] };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var balance = await userService.GetUserBalanceAsync(userId);

            // Assert
            balance.Should().Be(1234.56m);
        }
    }
}
