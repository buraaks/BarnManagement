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
        try
        {
            // Debug: Manuel olarak header ekle
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
            if (!string.IsNullOrEmpty(_jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            }
            
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Windows.Forms.MessageBox.Show($"GetUserProfile hatası:\nStatus: {response.StatusCode}\nToken var mı: {!string.IsNullOrEmpty(_jwtToken)}\nError: {error}", "API Debug");
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"GetUserProfile exception: {ex.Message}", "API Debug");
        }

        return null;
    }

    public async Task<List<FarmDto>> GetUserFarmsAsync()
    {
        try
        {
            // Debug: Manuel olarak header ekle
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/farms");
            if (!string.IsNullOrEmpty(_jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            }
            
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<FarmDto>>() ?? new List<FarmDto>();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Windows.Forms.MessageBox.Show($"GetUserFarms hatası:\nStatus: {response.StatusCode}\nToken var mı: {!string.IsNullOrEmpty(_jwtToken)}\nError: {error}", "API Debug");
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"GetUserFarms exception: {ex.Message}", "API Debug");
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

    // ==================== Animal Methods ====================

    public async Task<(AnimalDto? Animal, string? Error)> BuyAnimalAsync(Guid farmId, BuyAnimalRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/farms/{farmId}/animals/buy", request);

        if (response.IsSuccessStatusCode)
        {
            var animal = await response.Content.ReadFromJsonAsync<AnimalDto>();
            return (animal, null);
        }

        var error = await response.Content.ReadAsStringAsync();
        return (null, error);
    }

    public async Task<(AnimalDto? Animal, string? Error)> SellAnimalAsync(Guid animalId)
    {
        var response = await _httpClient.PostAsync($"/api/animals/{animalId}/sell", null);

        if (response.IsSuccessStatusCode)
        {
            var animal = await response.Content.ReadFromJsonAsync<AnimalDto>();
            return (animal, null);
        }

        var error = await response.Content.ReadAsStringAsync();
        return (null, error);
    }

    public async Task<List<AnimalDto>> GetFarmAnimalsAsync(Guid farmId)
    {
        var response = await _httpClient.GetAsync($"/api/farms/{farmId}/animals");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<AnimalDto>>() ?? new List<AnimalDto>();
        }

        return new List<AnimalDto>();
    }

    public async Task<AnimalDto?> GetAnimalByIdAsync(Guid animalId)
    {
        var response = await _httpClient.GetAsync($"/api/animals/{animalId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AnimalDto>();
        }

        return null;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_jwtToken);
}

