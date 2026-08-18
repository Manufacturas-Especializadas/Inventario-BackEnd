using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> AnyAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmployeeIdAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameWithRolesAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}