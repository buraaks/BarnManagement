using BarnManagement.Core.DTOs;

namespace BarnManagement.Core.Interfaces;

public interface IFarmService
{
    Task<FarmDto> CreateFarmAsync(CreateFarmRequest request, Guid ownerId);
    Task<IEnumerable<FarmDto>> GetUserFarmsAsync(Guid ownerId);
    Task<FarmDto?> GetFarmByIdAsync(Guid farmId, Guid ownerId);
    Task<FarmDto?> UpdateFarmAsync(Guid farmId, UpdateFarmRequest request, Guid ownerId);
    Task<bool> DeleteFarmAsync(Guid farmId, Guid ownerId);
}
