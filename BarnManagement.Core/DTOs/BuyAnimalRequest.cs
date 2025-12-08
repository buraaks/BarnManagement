namespace BarnManagement.Core.DTOs;

public record BuyAnimalRequest(
    string Species,
    string Name,
    decimal PurchasePrice,
    int LifeSpanDays,
    int ProductionInterval
);
