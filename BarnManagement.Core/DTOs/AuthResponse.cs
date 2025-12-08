namespace BarnManagement.Core.DTOs;

public record AuthResponse(string Token, DateTime Expiration);
