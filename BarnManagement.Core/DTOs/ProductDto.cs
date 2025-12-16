namespace BarnManagement.Core.DTOs;

public record ProductDto(
    Guid Id,
    string ProductType,
    decimal SalePrice,
    DateTime ProducedAt,
    int Quantity
);
