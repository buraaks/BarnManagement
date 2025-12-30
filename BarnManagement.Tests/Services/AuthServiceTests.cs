using BarnManagement.Business.Services;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using BarnManagement.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BarnManagement.Tests.Services
{
    public class AuthServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<IJwtTokenGenerator> _jwtMock;

        public AuthServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _jwtMock = new Mock<IJwtTokenGenerator>();
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUserAndDefaultFarm()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var authService = new AuthService(context, _jwtMock.Object);
            
            var email = $"test_{Guid.NewGuid()}@example.com";
            var username = "testuser";
            var request = new RegisterRequest(email, username, "Password123!");
            _jwtMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("test-token");

            // Act
            var response = await authService.RegisterAsync(request);

            // Assert
            response.Token.Should().Be("test-token");
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            user.Should().NotBeNull();
            user.Username.Should().Be(username);
            user.Balance.Should().Be(1000);

            var farm = await context.Farms.FirstOrDefaultAsync(f => f.OwnerId == user.Id);
            farm.Should().NotBeNull();
            farm.Name.Should().Contain(username);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_IfEmailExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var authService = new AuthService(context, _jwtMock.Object);

            var email = $"existing_{Guid.NewGuid()}@example.com";
            var user = new User { Email = email, Username = "existing", PasswordHash = new byte[0] };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var request = new RegisterRequest(email, "new", "password");

            // Act & Assert
            await authService.Invoking(s => s.RegisterAsync(request))
                .Should().ThrowAsync<Exception>().WithMessage("*zaten kullanımda*");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_ForValidCredentials()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var authService = new AuthService(context, _jwtMock.Object);

            var email = $"login_{Guid.NewGuid()}@example.com";
            var password = "Password123!";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Email = email, 
                Username = "loginuser", 
                PasswordHash = System.Text.Encoding.UTF8.GetBytes(hash) 
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            _jwtMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("login-token");

            var request = new LoginRequest(email, password);

            // Act
            var response = await authService.LoginAsync(request);

            // Assert
            response.Token.Should().Be("login-token");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_ForInvalidPassword()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var authService = new AuthService(context, _jwtMock.Object);

            var email = $"wrongpass_{Guid.NewGuid()}@example.com";
            var hash = BCrypt.Net.BCrypt.HashPassword("correct_password");
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Email = email, 
                Username = "wrongpass", 
                PasswordHash = System.Text.Encoding.UTF8.GetBytes(hash) 
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var request = new LoginRequest(email, "wrong_password");

            // Act & Assert
            await authService.Invoking(s => s.LoginAsync(request))
                .Should().ThrowAsync<Exception>().WithMessage("*hatalı*");
        }
    }
}
