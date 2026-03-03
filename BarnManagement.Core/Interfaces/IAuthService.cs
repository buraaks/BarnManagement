using BarnManagement.Core.DTOs;

namespace BarnManagement.Core.Interfaces;
// Kısa mimari özet: Bu arayüz, kimlik doğrulama kullanım senaryoları için servis sözleşmesini tanımlar.


public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
