using BarnManagement.UI.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BarnManagement.UI.Services;

// API ile iletişim kuran istemci sınıfı
public class BarnManagementApiClient
{
    private readonly HttpClient _httpClient;
    private string? _jwtToken;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public bool IsAuthenticated => !string.IsNullOrEmpty(_jwtToken);

    public BarnManagementApiClient(IConfiguration configuration)
    {
        var apiBaseUrl = configuration["AppSettings:ApiBaseUrl"] ?? "https://localhost:7067";
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
        };
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public void SetAuthToken(string token)
    {
        _jwtToken = token;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // Kullanıcı girişi yap ve token'ı sakla
    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var request = new { email, password };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            var authResponse = await JsonSerializer.DeserializeAsync<AuthResponse>(responseStream, _jsonOptions);
            
            if (authResponse != null)
            {
                SetAuthToken(authResponse.Token);
            }
            return authResponse;
        }

        return null;
    }

    // Yeni kullanıcı kaydı
    public async Task<bool> RegisterAsync(string email, string username, string password)
    {
        var request = new { email, username, password };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/auth/register", content);

        return response.IsSuccessStatusCode;
    }

    // Kullanıcı profilini getir
    public async Task<UserDto?> GetUserProfileAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
            if (!string.IsNullOrEmpty(_jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            }
            
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<UserDto>(responseStream, _jsonOptions);
            }
        }
        catch (Exception) { }

        return null;
    }

    // Kullanıcıya ait çiftlikleri listele
    public async Task<List<FarmDto>> GetUserFarmsAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/farms");
            if (!string.IsNullOrEmpty(_jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            }
            
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<FarmDto>>(responseStream, _jsonOptions) ?? new List<FarmDto>();
            }
        }
        catch (Exception) { }

        return new List<FarmDto>();
    }

    // Yeni çiftlik oluştur
    public async Task<FarmDto?> CreateFarmAsync(string farmName)
    {
        var request = new { name = farmName };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/farms", content);

        if (response.IsSuccessStatusCode)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<FarmDto>(responseStream, _jsonOptions);
        }

        return null;
    }

    // Hayvan satın al
    public async Task<(bool Success, string? Error)> BuyAnimalAsync(Guid farmId, BuyAnimalRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"/api/farms/{farmId}/animals/buy", content);

        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }
    
    // Çiftlikteki hayvanları listele
    public async Task<List<AnimalDto>> GetFarmAnimalsAsync(Guid farmId)
    {
        var response = await _httpClient.GetAsync($"/api/farms/{farmId}/animals");

        if (response.IsSuccessStatusCode)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<AnimalDto>>(responseStream, _jsonOptions) ?? new List<AnimalDto>();
        }

        return new List<AnimalDto>();
    }

    // Hayvan sat
    public async Task<(AnimalDto? Animal, string? Error)> SellAnimalAsync(Guid animalId)
    {
        var response = await _httpClient.PostAsync($"/api/animals/{animalId}/sell", null);

        if (response.IsSuccessStatusCode)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            var animal = await JsonSerializer.DeserializeAsync<AnimalDto>(responseStream, _jsonOptions);
            return (animal, null);
        }

        var error = await response.Content.ReadAsStringAsync();
        return (null, error);
    }

    public async Task<AnimalDto?> GetAnimalByIdAsync(Guid animalId)
    {
        var response = await _httpClient.GetAsync($"/api/animals/{animalId}");

        if (response.IsSuccessStatusCode)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<AnimalDto>(responseStream, _jsonOptions);
        }

        return null;
    }

    public async Task<List<ProductDto>> GetFarmProductsAsync(Guid farmId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/farms/{farmId}/products");

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<ProductDto>>(responseStream, _jsonOptions) ?? new List<ProductDto>();
            }
        }
        catch (Exception) { }

        return new List<ProductDto>();
    }

    // Ürün sat
    public async Task<(bool Success, string? Error)> SellProductAsync(Guid productId)
    {
        var response = await _httpClient.PostAsync($"/api/products/{productId}/sell", null);

        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    // Tüm ürünleri sat
    public async Task<(bool Success, decimal TotalEarnings, string? Error)> SellAllProductsAsync(Guid farmId)
    {
        var response = await _httpClient.PostAsync($"/api/farms/{farmId}/products/sell-all", null);

        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            if (doc.RootElement.TryGetProperty("totalEarnings", out var earningProp))
            {
                return (true, earningProp.GetDecimal(), null);
            }
            return (true, 0m, null);
        }

        var error = await response.Content.ReadAsStringAsync();
        return (false, 0m, error);
    }

    // Hesabı sıfırla
    public async Task<bool> ResetGameAsync()
    {
        var response = await _httpClient.PostAsync("/api/users/reset", null);
        return response.IsSuccessStatusCode;
    }
}
