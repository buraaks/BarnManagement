namespace BarnManagement.Core.DTOs;

public record UserDto(Guid Id, string Email, string Username, decimal Balance);
