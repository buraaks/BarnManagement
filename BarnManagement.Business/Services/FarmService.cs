using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarnManagement.Business.Services;

public class FarmService : IFarmService
{
    private readonly AppDbContext _context;

    public FarmService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FarmDto> CreateFarmAsync(CreateFarmRequest request, Guid ownerId)
    {
        var farm = new Farm
        {
            Name = request.Name,
            OwnerId = ownerId
        };

        _context.Farms.Add(farm);
        await _context.SaveChangesAsync();

        return new FarmDto(farm.Id, farm.Name, farm.OwnerId);
    }

    public async Task<IEnumerable<FarmDto>> GetUserFarmsAsync(Guid ownerId)
    {
        return await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => new FarmDto(f.Id, f.Name, f.OwnerId))
            .ToListAsync();
    }

    public async Task<FarmDto?> GetFarmByIdAsync(Guid farmId, Guid ownerId)
    {
        var farm = await _context.Farms.FindAsync(farmId);
        
        // Çiftlik bulunamadı veya kullanıcıya ait değil
        if (farm == null || farm.OwnerId != ownerId) return null;

        return new FarmDto(farm.Id, farm.Name, farm.OwnerId);
    }

    public async Task<FarmDto?> UpdateFarmAsync(Guid farmId, UpdateFarmRequest request, Guid ownerId)
    {
        var farm = await _context.Farms.FindAsync(farmId);
        
        // Çiftlik bulunamadı veya kullanıcıya ait değil
        if (farm == null || farm.OwnerId != ownerId) return null;

        farm.Name = request.Name;
        await _context.SaveChangesAsync();

        return new FarmDto(farm.Id, farm.Name, farm.OwnerId);
    }

    public async Task<bool> DeleteFarmAsync(Guid farmId, Guid ownerId)
    {
        var farm = await _context.Farms.FindAsync(farmId);

        // Çiftlik bulunamadı veya kullanıcıya ait değil
        if (farm == null || farm.OwnerId != ownerId) return false;

        _context.Farms.Remove(farm);
        await _context.SaveChangesAsync();

        return true;
    }
}
