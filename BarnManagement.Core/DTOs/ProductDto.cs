namespace BarnManagement.Core.DTOs;
// Kısa mimari özet: Bu DTO, katmanlar arası veri taşımak ve API sözleşmesini sade/tutarlı tutmak için kullanılır.


public record ProductDto(
    Guid Id,
    string ProductType,
    decimal SalePrice,
    DateTime ProducedAt,
    int Quantity
);
