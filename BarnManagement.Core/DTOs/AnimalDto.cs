using BarnManagement.Core.Enums;

namespace BarnManagement.Core.DTOs;
// Kısa mimari özet: Bu DTO, katmanlar arası veri taşımak ve API sözleşmesini sade/tutarlı tutmak için kullanılır.


public record AnimalDto(
    Guid Id,
    Guid FarmId,
    AnimalSpecies Species,
    string Name,
    DateTime BirthDate,
    int LifeSpanDays,
    int ProductionInterval,
    DateTime? NextProductionAt,
    decimal PurchasePrice,
    decimal SellPrice
);
