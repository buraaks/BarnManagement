using BarnManagement.Core.Enums;

public record BuyAnimalRequest(
    AnimalSpecies Species,
    string Name,
    decimal PurchasePrice,
    int ProductionInterval
);
