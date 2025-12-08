using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BarnManagement.Business.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(AppDbContext context, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        // Convert stored byte[] hash back to string for BCrypt
        string storedHash = System.Text.Encoding.UTF8.GetString(user.PasswordHash);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, storedHash))
        {
            throw new Exception("Invalid credentials");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        
        // Expiration is hardcoded in generator for now, aiming for simplicity. 
        // In a real scenario we'd get the expiration from the generator or config.
        return new AuthResponse(token, DateTime.UtcNow.AddMinutes(60)); 
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new Exception("Email already exists");
        }

        // Hashing password
        // Note: Check if PasswordHash in DB is byte[] or string. 
        // Project.md says byte[], Entity says byte[].
        // BCrypt returns string. We need to store it properly.
        // Wait, standard BCrypt returns a string hash. 
        // If DB expects byte[], we need to convert.
        // Let's assume we can store bytes.
        
        var passwordHashString = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var passwordHashBytes = System.Text.Encoding.UTF8.GetBytes(passwordHashString);

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = passwordHashBytes,
            Balance = 0, // Initial balance
            Farms = new List<Farm>()
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, DateTime.UtcNow.AddMinutes(60));
    }
}
