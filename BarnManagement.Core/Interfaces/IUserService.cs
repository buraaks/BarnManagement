using BarnManagement.Core.DTOs;

namespace BarnManagement.Core.Interfaces;
// Kısa mimari özet: Bu arayüz, kullanıcı işlemleri için servis katmanının dışa açık sözleşmesini tanımlar.


public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<decimal> GetUserBalanceAsync(Guid userId);
    Task ResetAccountAsync(Guid userId);
}
