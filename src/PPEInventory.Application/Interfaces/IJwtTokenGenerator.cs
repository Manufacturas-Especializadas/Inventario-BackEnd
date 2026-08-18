using PPEInventory.Application.Common.Models;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(
        User user,
        IReadOnlyCollection<string> roles);
}