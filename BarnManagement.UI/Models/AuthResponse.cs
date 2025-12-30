namespace BarnManagement.UI.Models;

public record AuthResponse(string Token, DateTimeOffset Expiration);
