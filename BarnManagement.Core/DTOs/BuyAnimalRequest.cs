using BarnManagement.Core.Enums;

// Kısa mimari özet: Bu DTO, katmanlar arası veri taşımak ve API sözleşmesini sade/tutarlı tutmak için kullanılır.

public record BuyAnimalRequest(
    AnimalSpecies Species,
    string Name,
    decimal PurchasePrice,
    int ProductionInterval
);
