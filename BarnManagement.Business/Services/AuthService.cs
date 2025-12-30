using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

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
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("Giriş bilgileri hatalı.");
        }
        
        // Saklanan byte[] hash bilgisini BCrypt doğrulaması için string formatına dönüştürün
        string storedHash = System.Text.Encoding.UTF8.GetString(user.PasswordHash);

        if (!BCrypt.Net.BCrypt.Verify(request.Password, storedHash))
        {
            throw new UnauthorizedAccessException("Giriş bilgileri hatalı.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        
        // Sadelik açısından geçerlilik süresi şimdilik üreteç içinde sabitlenmiştir.
        // Gerçek bir senaryoda bu süreyi üreteçten veya yapılandırmadan alırdık.
        return new AuthResponse(token, DateTime.UtcNow.AddMinutes(60)); 
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new InvalidOperationException("Bu e-posta adresi zaten kullanımda.");
        }

        // Parolayı hash'leme
        var passwordHashString = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var passwordHashBytes = System.Text.Encoding.UTF8.GetBytes(passwordHashString);

        var user = new User
        {
            Id = Guid.NewGuid(), // Farm için kullanılabilir olduğundan emin olmak için Id'yi manuel olarak atayın
            Email = request.Email,
            Username = request.Username,
            PasswordHash = passwordHashBytes,
            Balance = 1000,
            Farms = new List<Farm>()
        };

        // Otomatik olarak bir çiftlik oluştur
        var defaultFarm = new Farm
        {
            Id = Guid.NewGuid(),
            Name = $"{request.Username}'ın Çiftliği",
            OwnerId = user.Id
        };
        
        _context.Users.Add(user);
        _context.Farms.Add(defaultFarm);
        
        await _context.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, DateTime.UtcNow.AddMinutes(60));
    }
}
