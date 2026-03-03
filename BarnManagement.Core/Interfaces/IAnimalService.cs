using BarnManagement.Core.DTOs;

namespace BarnManagement.Core.Interfaces;
// Kısa mimari özet: Bu arayüz, hayvan yönetimi ve ilgili iş akışları için servis sözleşmesini tanımlar.


public interface IAnimalService
{
    Task<AnimalDto> BuyAnimalAsync(Guid farmId, BuyAnimalRequest request, Guid userId);
    Task<AnimalDto?> SellAnimalAsync(Guid animalId, Guid userId);
    Task<IEnumerable<AnimalDto>> GetFarmAnimalsAsync(Guid farmId, Guid userId);
    Task<AnimalDto?> GetAnimalByIdAsync(Guid animalId, Guid userId);
}
