using BarnManagement.Core.DTOs;
using BarnManagement.Tests.Integration;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BarnManagement.Tests.Integration
{
    public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UsersControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _factory.InitializeDatabase();
        }

        private async Task<HttpClient> GetAuthenticatedClientAsync(string email)
        {
            var client = _factory.CreateClient();
            
            // Register
            var regReq = new RegisterRequest(email, "UserTest", "Pass123!");
            await client.PostAsJsonAsync("/api/auth/register", regReq);

            // Login
            var loginReq = new LoginRequest(email, "Pass123!");
            var logRes = await client.PostAsJsonAsync("/api/auth/login", loginReq);
            var authRes = await logRes.Content.ReadFromJsonAsync<AuthResponse>();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authRes!.Token);
            return client;
        }

        [Fact]
        public async Task GetProfile_ShouldReturnUserInfo()
        {
            // Arrange
            var email = "profile@test.com";
            var client = await GetAuthenticatedClientAsync(email);

            // Act
            var response = await client.GetAsync("/api/users/me");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            user.Should().NotBeNull();
            user!.Email.Should().Be(email);
        }

        [Fact]
        public async Task GetBalance_ShouldReturnCorrectBalance()
        {
            // Arrange
            var email = "balance@test.com";
            var client = await GetAuthenticatedClientAsync(email);

            // Act
            var response = await client.GetAsync("/api/users/me/balance");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, decimal>>();
            result!["balance"].Should().Be(1000); // Varsayılan başlangıç bakiyesi
        }

        [Fact]
        public async Task ResetAccount_ShouldSucceed()
        {
            // Arrange
            var email = "reset@test.com";
            var client = await GetAuthenticatedClientAsync(email);

            // Act
            var response = await client.PostAsync("/api/users/reset", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("başarıyla");
        }
    }
}
