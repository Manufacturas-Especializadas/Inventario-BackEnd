using PPEInventory.Application.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace PPEInventory.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BC.HashPassword(password);
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        return BC.Verify(
            password,
            passwordHash);
    }
}