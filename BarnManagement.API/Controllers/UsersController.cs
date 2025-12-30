using System.Security.Claims;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedAccessException();
        }
        return userId;
    }

    // Kendi profil bilgilerini getir
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMyProfile()
    {
        var userId = GetUserId();
        var user = await _userService.GetUserByIdAsync(userId);
        
        if (user == null) return NotFound("Kullanıcı bulunamadı.");
        
        return Ok(user);
    }

    // ID ile kullanıcı getir
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound("Kullanıcı bulunamadı.");
        return Ok(user);
    }
    // Hesabı sıfırla
    [HttpPost("reset")]
    public async Task<IActionResult> ResetAccount()
    {
        try
        {
            var userId = GetUserId();
            await _userService.ResetAccountAsync(userId);
            return Ok(new { message = "Hesap başarıyla sıfırlandı." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
