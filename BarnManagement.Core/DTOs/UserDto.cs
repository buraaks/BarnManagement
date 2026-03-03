namespace BarnManagement.Core.DTOs;
// Kısa mimari özet: Bu DTO, katmanlar arası veri taşımak ve API sözleşmesini sade/tutarlı tutmak için kullanılır.


public record UserDto(Guid Id, string Email, string Username, decimal Balance);
