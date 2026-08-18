using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Role>> GetByNamesAsync(
        IReadOnlyCollection<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(x =>
                roleNames.Contains(x.Name) &&
                x.IsActive)
            .ToListAsync(cancellationToken);
    }
}