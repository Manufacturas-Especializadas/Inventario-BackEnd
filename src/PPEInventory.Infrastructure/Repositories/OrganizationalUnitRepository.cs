using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class OrganizationalUnitRepository
    : IOrganizationalUnitRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationalUnitRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrganizationalUnit>>
        GetAllAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context
            .OrganizationalUnits
            .AsNoTracking()
            .Include(x => x.Parent)
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<OrganizationalUnit?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context
            .OrganizationalUnits
            .AsNoTracking()
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByParentAndNameAsync(
        int? parentId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return _context
            .OrganizationalUnits
            .AnyAsync(
                x =>
                    x.ParentId == parentId &&
                    x.Name == name,
                cancellationToken);
    }

    public async Task<IReadOnlyList<OrganizationalUnit>>
        GetPathToRootAsync(
            int organizationalUnitId,
            CancellationToken cancellationToken = default)
    {
        var result =
            new List<OrganizationalUnit>();

        var visitedIds =
            new HashSet<int>();

        int? currentId =
            organizationalUnitId;

        while (currentId.HasValue)
        {
            if (!visitedIds.Add(currentId.Value))
            {
                throw new InvalidOperationException(
                    "A cycle was detected in the organizational structure.");
            }

            var unit =
                await _context
                    .OrganizationalUnits
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                            currentId.Value,
                        cancellationToken);

            if (unit is null)
            {
                break;
            }

            result.Add(unit);

            currentId =
                unit.ParentId;
        }

        return result;
    }

    public async Task AddAsync(
        OrganizationalUnit organizationalUnit,
        CancellationToken cancellationToken = default)
    {
        await _context
            .OrganizationalUnits
            .AddAsync(
                organizationalUnit,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }
}