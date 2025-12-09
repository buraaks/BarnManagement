using BarnManagement.Core.DTOs;

namespace BarnManagement.Core.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<decimal> GetUserBalanceAsync(Guid userId);
    Task ResetAccountAsync(Guid userId);
}
