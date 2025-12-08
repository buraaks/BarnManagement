using System.Security.Claims;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _animalService;

    public AnimalsController(IAnimalService animalService)
    {
        _animalService = animalService;
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

    /// <summary>
    /// Hayvan satın alma
    /// </summary>
    [HttpPost("farms/{farmId}/animals/buy")]
    public async Task<ActionResult<AnimalDto>> BuyAnimal(Guid farmId, BuyAnimalRequest request)
    {
        try
        {
            var userId = GetUserId();
            var animal = await _animalService.BuyAnimalAsync(farmId, request, userId);
            return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, animal);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("You do not own this farm.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Hayvan satma
    /// </summary>
    [HttpPost("animals/{id}/sell")]
    public async Task<ActionResult<AnimalDto>> SellAnimal(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var animal = await _animalService.SellAnimalAsync(id, userId);

            if (animal == null) return NotFound("Animal not found.");

            return Ok(animal);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("You do not own this animal.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Çiftliğin hayvanlarını listele
    /// </summary>
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

    /// <summary>
    /// Hayvan detayı
    /// </summary>
    [HttpGet("animals/{id}")]
    public async Task<ActionResult<AnimalDto>> GetAnimalById(Guid id)
    {
        var animal = await _animalService.GetAnimalByIdAsync(id);
        if (animal == null) return NotFound("Animal not found.");
        return Ok(animal);
    }
}
