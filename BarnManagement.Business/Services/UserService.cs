using BarnManagement.Core.DTOs;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarnManagement.Business.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetUserBalanceAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        return user.Balance;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserDto(user.Id, user.Email, user.Username, user.Balance);
    }

    public async Task ResetAccountAsync(Guid userId)
    {
        // SQLite In-Memory has issues with multiple SaveChanges inside a transaction in some async contexts.
        // We combine operations or handle them carefully.
        
        // 1. Ürünleri Sil
        var products = await _context.Products.Where(p => p.Farm.OwnerId == userId).ToListAsync();
        _context.Products.RemoveRange(products);

        // 2. Hayvanları Sil
        var animals = await _context.Animals.Where(a => a.Farm.OwnerId == userId).ToListAsync();
        _context.Animals.RemoveRange(animals);

        // 3. Çiftlikleri Sil
        var farms = await _context.Farms.Where(f => f.OwnerId == userId).ToListAsync();
        _context.Farms.RemoveRange(farms);

        // 4. Kullanıcı Bakiyesini Sıfırla
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Balance = 5000m;
        }

        // 5. Varsayılan Çiftlik Oluştur
        var defaultFarm = new Farm { OwnerId = userId, Name = "My Farm" };
        _context.Farms.Add(defaultFarm);
        
        await _context.SaveChangesAsync();
    }
}
