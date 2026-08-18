using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> AnyAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Users.AnyAsync(cancellationToken);
    }

    public Task<bool> ExistsByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return _context.Users.AnyAsync(
            x => x.Username == username,
            cancellationToken);
    }

    public Task<bool> ExistsByEmployeeIdAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        return _context.Users.AnyAsync(
            x => x.EmployeeId == employeeId,
            cancellationToken);
    }

    public Task<User?> GetByUsernameWithRolesAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .Include(x => x.Employee)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(
                x => x.Username == username,
                cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(
            user,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}