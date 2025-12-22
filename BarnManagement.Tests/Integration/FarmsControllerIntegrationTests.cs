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

namespace BarnManagement.Tests.Integration
{
    public class FarmsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public FarmsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _factory.InitializeDatabase();
        }

        private async Task<string> GetTokenAsync(HttpClient client, string email)
        {
            var registerRequest = new RegisterRequest(email, "FarmUser", "Pass123!");
            await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            var loginRequest = new LoginRequest(email, "Pass123!");
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return authResponse!.Token;
        }

        [Fact]
        public async Task CreateFarm_ShouldSucceed_WithValidToken()
        {
            // Arrange
            var client = _factory.CreateClient();
            var token = await GetTokenAsync(client, $"owner_{Guid.NewGuid()}@test.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new CreateFarmRequest("Test Integration Farm");

            // Act
            var response = await client.PostAsJsonAsync("/api/farms", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var farm = await response.Content.ReadFromJsonAsync<FarmDto>();
            farm.Should().NotBeNull();
            farm!.Name.Should().Be("Test Integration Farm");
        }

        [Fact]
        public async Task UpdateFarm_ShouldSucceed()
        {
            // Arrange
            var client = _factory.CreateClient();
            var token = await GetTokenAsync(client, $"update_owner_{Guid.NewGuid()}@test.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Register default farm oluşturduğu için önce listeyi alalım
            var listResponse = await client.GetAsync("/api/farms");
            var farms = await listResponse.Content.ReadFromJsonAsync<List<FarmDto>>();
            var farmToUpdate = farms![0];

            var updateRequest = new UpdateFarmRequest("Updated Farm Name");

            // Act
            var response = await client.PutAsJsonAsync($"/api/farms/{farmToUpdate.Id}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = await response.Content.ReadFromJsonAsync<FarmDto>();
            updated!.Name.Should().Be("Updated Farm Name");
        }

        [Fact]
        public async Task DeleteFarm_ShouldSucceed()
        {
            // Arrange
            var client = _factory.CreateClient();
            var token = await GetTokenAsync(client, $"delete_owner_{Guid.NewGuid()}@test.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var listResponse = await client.GetAsync("/api/farms");
            var farms = await listResponse.Content.ReadFromJsonAsync<List<FarmDto>>();
            var farmToDelete = farms![0];

            // Act
            var response = await client.DeleteAsync($"/api/farms/{farmToDelete.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetFarms_ShouldReturnList()
        {
            // Arrange
            var client = _factory.CreateClient();
            var token = await GetTokenAsync(client, $"list_{Guid.NewGuid()}@test.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // API'nin RegisterAsync metodu zaten bir varsayılan çiftlik oluşturuyor.
            // Bu yüzden liste en az 1 eleman içermeli.

            // Act
            var response = await client.GetAsync("/api/farms");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var farms = await response.Content.ReadFromJsonAsync<List<FarmDto>>();
            farms.Should().NotBeNull();
            farms.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Access_WithoutToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/farms");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
