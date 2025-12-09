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
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Ürünleri Sil (Hayvan -> Çiftlik -> Kullanıcı ilişkisi üzerinden)
            // SQL: DELETE FROM Products WHERE AnimalId IN (SELECT Id FROM Animals WHERE FarmId IN (SELECT Id FROM Farms WHERE OwnerId = @userId))
            await _context.Products
                .Where(p => p.Animal.Farm.OwnerId == userId)
                .ExecuteDeleteAsync();

            // 2. Hayvanları Sil
            await _context.Animals
                .Where(a => a.Farm.OwnerId == userId)
                .ExecuteDeleteAsync();

            // 3. Çiftlikleri Sil
            await _context.Farms
                .Where(f => f.OwnerId == userId)
                .ExecuteDeleteAsync();

            // 4. Kullanıcı Bakiyesini Sıfırla (Örn: 5000)
            await _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.Balance, 5000m));
            
            // 5. Varsayılan Çiftlik Oluştur
            var defaultFarm = new Farm { OwnerId = userId, Name = "My Farm" };
            _context.Farms.Add(defaultFarm);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
