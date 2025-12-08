namespace BarnManagement.UI.Models;

public record BuyAnimalRequest(
    string Species,
    string Name,
    decimal PurchasePrice,
    int LifeSpanDays,
    int ProductionInterval
);
