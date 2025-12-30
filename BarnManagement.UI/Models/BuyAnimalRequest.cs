namespace BarnManagement.UI.Models;

public record BuyAnimalRequest(
    AnimalSpecies Species,
    string Name,
    decimal PurchasePrice,
    int ProductionInterval
);
