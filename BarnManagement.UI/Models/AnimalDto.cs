namespace BarnManagement.UI.Models;

public record AnimalDto(
    Guid Id,
    Guid FarmId,
    string Species,
    string Name,
    DateTime BirthDate,
    int LifeSpanDays,
    int ProductionInterval,
    DateTime? NextProductionAt,
    decimal PurchasePrice,
    decimal SellPrice
);
