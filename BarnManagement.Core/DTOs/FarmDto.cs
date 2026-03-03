namespace BarnManagement.Core.DTOs;
// Kısa mimari özet: Bu DTO, katmanlar arası veri taşımak ve API sözleşmesini sade/tutarlı tutmak için kullanılır.


public record FarmDto(Guid Id, string Name, Guid OwnerId);
