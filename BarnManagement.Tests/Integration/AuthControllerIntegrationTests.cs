using BarnManagement.Core.DTOs;
using BarnManagement.Tests.Integration;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BarnManagement.Tests.Integration
{
    // Program sınıfına erişebilmek için API projesinin referans alınmış olması gerekir.
    // .NET 6+ Minimal API'lerde Program sınıfı gizli (internal) olabilir, 
    // bunu API projesindeki Program.cs sonuna 'public partial class Program { }' ekleyerek aşabiliriz.
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AuthControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _factory.InitializeDatabase();
        }

        [Fact]
        public async Task Register_And_Login_ShouldSucceed()
        {
            // Arrange
            var client = _factory.CreateClient();
            var email = $"integration_{Guid.NewGuid()}@test.com";
            var registerRequest = new RegisterRequest(email, "IntUser", "Password123!");

            // Act - Register
            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Assert - Register
            registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
            authResponse.Should().NotBeNull();
            authResponse!.Token.Should().NotBeNullOrEmpty();

            // Act - Login
            var loginRequest = new LoginRequest(email, "Password123!");
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert - Login
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginAuthResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            loginAuthResponse.Should().NotBeNull();
            loginAuthResponse!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldFail_WithWrongPassword()
        {
            // Arrange
            var client = _factory.CreateClient();
            var email = $"wrongpass_{Guid.NewGuid()}@test.com";
            var registerRequest = new RegisterRequest(email, "WrongPassUser", "Password123!");
            await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Act
            var loginRequest = new LoginRequest(email, "IncorrectPassword");
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
