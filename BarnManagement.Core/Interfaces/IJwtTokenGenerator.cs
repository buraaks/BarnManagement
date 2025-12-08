using System.Security.Claims;
using BarnManagement.Core.Entities;

namespace BarnManagement.Core.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
