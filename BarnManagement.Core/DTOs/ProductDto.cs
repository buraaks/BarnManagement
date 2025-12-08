namespace BarnManagement.Core.DTOs;

public record ProductDto(
    Guid Id,
    Guid AnimalId,
    string ProductType,
    decimal SalePrice,
    DateTime ProducedAt,
    bool IsSold,
    DateTime? SoldAt
);
