using System.Security.Claims;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;
// Kısa mimari özet: Bu controller, hayvan yönetimi işlevlerini API endpointleri olarak sunar ve servis katmanına delegasyon yapar.


// [Authorize]: Bu controller'daki tüm endpoint'ler için geçerli bir JWT token zorunludur.
[Authorize]
[ApiController]
[Route("api")]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _animalService;
    private readonly ISseBroadcaster _broadcaster;

    // Bağımlılık Enjeksiyonu: AnimalService constructor üzerinden alınır.
    public AnimalsController(IAnimalService animalService, ISseBroadcaster broadcaster)
    {
        _animalService = animalService;
        _broadcaster = broadcaster;
    }

    // Yardımcı Metot: Token içerisindeki 'NameIdentifier' claim'inden kullanıcı ID'sini çeker.
    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedAccessException();
        }
        return userId;
    }

    // Çiftliğe hayvan satın al
    [HttpPost("farms/{farmId}/animals/buy")]
    public async Task<ActionResult<AnimalDto>> BuyAnimal(Guid farmId, BuyAnimalRequest request)
    {
        try
        {
            var userId = GetUserId(); // İsteği yapan kullanıcıyı bul
            var animal = await _animalService.BuyAnimalAsync(farmId, request, userId);

            await _broadcaster.BroadcastUpdateAsync("refresh", "animal_buy");

            // Başarılı olursa 201 Created döner ve yeni hayvanın bilgilerini verir.
            return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, animal);
        }
        catch (UnauthorizedAccessException)
        {
            // Kullanıcı bu çiftliğin sahibi değilse
            return Unauthorized("Bu çiftliğin sahibi değilsiniz.");
        }
        catch (InvalidOperationException ex)
        {
            // Bakiye yetersizliği gibi iş mantığı hataları
            return BadRequest(ex.Message);
        }
    }


    // Hayvan satışı yap
    [HttpPost("animals/{id}/sell")]
    public async Task<ActionResult<AnimalDto>> SellAnimal(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var animal = await _animalService.SellAnimalAsync(id, userId);

            if (animal == null) return NotFound("Hayvan bulunamadı.");

            await _broadcaster.BroadcastUpdateAsync("refresh", "animal_sell");

            return Ok(animal);
        }
        catch (UnauthorizedAccessException)
        {
            // Kullanıcı bu hayvanın sahibi değilse
            return Unauthorized("Bu hayvanın sahibi değilsiniz.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // Çiftlikteki hayvanları listele
    [HttpGet("farms/{farmId}/animals")]
    public async Task<ActionResult<IEnumerable<AnimalDto>>> GetFarmAnimals(Guid farmId)
    {
        try
        {
            var userId = GetUserId();
            var animals = await _animalService.GetFarmAnimalsAsync(farmId, userId);
            return Ok(animals);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }


    // ID ile hayvan getir
    [HttpGet("animals/{id}")]
    public async Task<ActionResult<AnimalDto>> GetAnimalById(Guid id)
    {
        var userId = GetUserId();
        var animal = await _animalService.GetAnimalByIdAsync(id, userId);
        if (animal == null) return NotFound("Hayvan bulunamadı veya bu hayvana erişim yetkiniz yok.");
        return Ok(animal);
    }
}
