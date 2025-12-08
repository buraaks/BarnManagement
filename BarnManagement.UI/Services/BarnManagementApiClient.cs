using BarnManagement.UI.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BarnManagement.UI.Services;

public class BarnManagementApiClient
{
    private readonly HttpClient _httpClient;
    private string? _jwtToken;

    public BarnManagementApiClient(IConfiguration configuration)
    {
        var apiBaseUrl = configuration["AppSettings:ApiBaseUrl"] ?? "http://localhost:5193";
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
        };
    }

    public void SetAuthToken(string token)
    {
        _jwtToken = token;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var request = new { email, password };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse != null)
            {
                SetAuthToken(authResponse.Token);
            }
            return authResponse;
        }

        return null;
    }

    public async Task<bool> RegisterAsync(string email, string username, string password)
    {
        var request = new { email, username, password };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

        return response.IsSuccessStatusCode;
    }

    public async Task<UserDto?> GetUserProfileAsync()
    {
        var response = await _httpClient.GetAsync("/api/users/me");
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        return null;
    }

    public async Task<List<FarmDto>> GetUserFarmsAsync()
    {
        var response = await _httpClient.GetAsync("/api/farms");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<FarmDto>>() ?? new List<FarmDto>();
        }

        return new List<FarmDto>();
    }

    public async Task<FarmDto?> CreateFarmAsync(string farmName)
    {
        var request = new { name = farmName };
        var response = await _httpClient.PostAsJsonAsync("/api/farms", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<FarmDto>();
        }

        return null;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_jwtToken);
}
