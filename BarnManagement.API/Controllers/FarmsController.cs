using System.Security.Claims;
using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FarmsController : ControllerBase
{
    private readonly IFarmService _farmService;

    public FarmsController(IFarmService farmService)
    {
        _farmService = farmService;
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

    [HttpPost]
    public async Task<ActionResult<FarmDto>> CreateFarm(CreateFarmRequest request)
    {
        try
        {
            var userId = GetUserId();
            var farm = await _farmService.CreateFarmAsync(request, userId);
            return CreatedAtAction(nameof(GetFarmById), new { id = farm.Id }, farm);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FarmDto>>> GetUserFarms()
    {
        try
        {
            var userId = GetUserId();
            var farms = await _farmService.GetUserFarmsAsync(userId);
            return Ok(farms);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FarmDto>> GetFarmById(Guid id)
    {
        var farm = await _farmService.GetFarmByIdAsync(id);
        if (farm == null) return NotFound();
        return Ok(farm);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FarmDto>> UpdateFarm(Guid id, UpdateFarmRequest request)
    {
        try
        {
            var userId = GetUserId();
            var updatedFarm = await _farmService.UpdateFarmAsync(id, request, userId);
            
            if (updatedFarm == null) return NotFound("Farm not found or you do not have permission to modify it.");
            
            return Ok(updatedFarm);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFarm(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var result = await _farmService.DeleteFarmAsync(id, userId);

            if (!result) return NotFound("Farm not found or you do not have permission to delete it.");

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
