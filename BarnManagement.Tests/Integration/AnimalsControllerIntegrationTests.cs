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
    public class AnimalsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AnimalsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _factory.InitializeDatabase();
        }

        private async Task<(HttpClient client, string token, Guid farmId)> SetupUserAndFarmAsync()
        {
            var client = _factory.CreateClient();
            var email = $"animal_owner_{Guid.NewGuid()}@test.com";
            
            // Register
            var regReq = new RegisterRequest(email, "AnimalUser", "Pass123!");
            await client.PostAsJsonAsync("/api/auth/register", regReq);

            // Login
            var logReq = new LoginRequest(email, "Pass123!");
            var logRes = await client.PostAsJsonAsync("/api/auth/login", logReq);
            var authRes = await logRes.Content.ReadFromJsonAsync<AuthResponse>();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authRes!.Token);

            // Get default farm
            var farmsRes = await client.GetAsync("/api/farms");
            var farms = await farmsRes.Content.ReadFromJsonAsync<List<FarmDto>>();
            
            return (client, authRes.Token, farms![0].Id);
        }

        [Fact]
        public async Task BuyAnimal_ShouldSucceed_WhenBalanceIsEnough()
        {
            // Arrange
            var (client, _, farmId) = await SetupUserAndFarmAsync();
            var request = new BuyAnimalRequest("Cow", "Daisy", 500, 300); // 500 para, 300 saniye interval

            // Act
            var response = await client.PostAsJsonAsync($"/api/farms/{farmId}/animals/buy", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var animal = await response.Content.ReadFromJsonAsync<AnimalDto>();
            animal.Should().NotBeNull();
            animal!.Species.Should().Be("Cow");
            animal.PurchasePrice.Should().Be(500);
        }

        [Fact]
        public async Task GetAnimals_ShouldReturnList()
        {
            // Arrange
            var (client, _, farmId) = await SetupUserAndFarmAsync();
            
            // Önce bir hayvan alalım
            var buyReq = new BuyAnimalRequest("Chicken", "Chick", 100, 60);
            await client.PostAsJsonAsync($"/api/farms/{farmId}/animals/buy", buyReq);

            // Act
            var response = await client.GetAsync($"/api/farms/{farmId}/animals");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var animals = await response.Content.ReadFromJsonAsync<List<AnimalDto>>();
            animals.Should().NotBeNull();
            animals.Should().Contain(a => a.Species == "Chicken");
        }
    }
}
