using System.Security.Claims;
using BarnManagement.Core.Entities;

namespace BarnManagement.Core.Interfaces;
// Kısa mimari özet: Bu arayüz, token üretim mekanizmasını soyutlayarak bağımlılıkların gevşek bağlı kalmasını sağlar.


public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
