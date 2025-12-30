using System.Security.Claims;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

// Bu controller'daki işlemler için giriş yapılmış olması şarttır.
[Authorize]
[ApiController]
[Route("api/farms")]
public class FarmsController : ControllerBase
{
    private readonly IFarmService _farmService;

    public FarmsController(IFarmService farmService)
    {
        _farmService = farmService;
    }

    // Token içerisinden o anki kullanıcının ID'sini çeken yardımcı metot.
    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedAccessException();
        }
        return userId;
    }

    // Kullanıcı çiftliklerini listele
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FarmDto>>> GetMyFarms()
    {
        var userId = GetUserId();
        var farms = await _farmService.GetUserFarmsAsync(userId);
        return Ok(farms);
    }

    // ID ile çiftlik getir
    [HttpGet("{id}")]
    public async Task<ActionResult<FarmDto>> GetFarmById(Guid id)
    {
        var farm = await _farmService.GetFarmByIdAsync(id);
        if (farm == null) return NotFound("Çiftlik bulunamadı.");
        return Ok(farm);
    }

    // Yeni çiftlik oluştur
    [HttpPost]
    public async Task<ActionResult<FarmDto>> CreateFarm(CreateFarmRequest request)
    {
        var userId = GetUserId(); // Çiftlik kime ait olacak? Tabii ki isteği yapana.
        var farm = await _farmService.CreateFarmAsync(request, userId);
        return CreatedAtAction(nameof(GetFarmById), new { id = farm.Id }, farm);
    }

    // Çiftlik güncelle
    [HttpPut("{id}")]
    public async Task<ActionResult<FarmDto>> UpdateFarm(Guid id, UpdateFarmRequest request)
    {
        try
        {
            var userId = GetUserId();
            var farm = await _farmService.UpdateFarmAsync(id, request, userId);
            return Ok(farm);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Bu çiftliği güncelleme yetkiniz yok.");
        }
    }

    // Çiftlik sil
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFarm(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var result = await _farmService.DeleteFarmAsync(id, userId);
            if (!result) return NotFound("Çiftlik bulunamadı.");

            return NoContent(); // 204 No Content (Başarıyla silindi)
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Bu çiftliği silme yetkiniz yok.");
        }
    }
}
