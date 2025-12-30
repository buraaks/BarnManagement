using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

[ApiController]
[Route("api/auth")] // Tüm istekler /api/auth ile başlar
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    // AuthService constructor üzerinden sisteme dahil edilir.
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Yeni hesap oluştur
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            // Kayıt başarılı: Token ve kullanıcı bilgileri döner.
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            // Email zaten kullanılıyorsa vb.
            return BadRequest(ex.Message);
        }
    }

    // Kullanıcı girişi ve token alımı

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            // Giriş başarılı: Kullanıcıya oturum için JWT token verilir.
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Şifre veya Email yanlışsa
            return Unauthorized(ex.Message);
        }
    }
}
