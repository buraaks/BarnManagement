namespace BarnManagement.UI.Models;

public record ProductDto(
    Guid Id,
    Guid AnimalId,
    string ProductType,
    decimal SalePrice,
    DateTime ProducedAt,
    int Quantity
);
