using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class OrganizationalUnitPPELimitRepository
    : IOrganizationalUnitPPELimitRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationalUnitPPELimitRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<
        OrganizationalUnitPPELimit>>
        GetAllAsync(
            int? organizationalUnitId,
            int? ppeProductId,
            CancellationToken cancellationToken = default)
    {
        var query =
            _context
                .OrganizationalUnitPPELimits
                .AsNoTracking()
                .Include(
                    x => x.OrganizationalUnit)
                .Include(
                    x => x.PPEProduct)
                .AsQueryable();

        if (organizationalUnitId.HasValue)
        {
            query =
                query.Where(
                    x =>
                        x.OrganizationalUnitId ==
                        organizationalUnitId.Value);
        }

        if (ppeProductId.HasValue)
        {
            query =
                query.Where(
                    x =>
                        x.PPEProductId ==
                        ppeProductId.Value);
        }

        return await query
            .OrderBy(
                x =>
                    x.OrganizationalUnit.Name)
            .ThenBy(
                x =>
                    x.PPEProduct.Name)
            .ToListAsync(
                cancellationToken);
    }

    public Task<OrganizationalUnitPPELimit?>
        GetByUnitAndProductAsync(
            int organizationalUnitId,
            int ppeProductId,
            CancellationToken cancellationToken = default)
    {
        return _context
            .OrganizationalUnitPPELimits
            .FirstOrDefaultAsync(
                x =>
                    x.OrganizationalUnitId ==
                    organizationalUnitId &&
                    x.PPEProductId ==
                    ppeProductId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<
        OrganizationalUnitPPELimit>>
        GetActiveByUnitsAndProductsAsync(
            IReadOnlyCollection<int>
                organizationalUnitIds,
            IReadOnlyCollection<int>
                ppeProductIds,
            CancellationToken cancellationToken = default)
    {
        return await _context
            .OrganizationalUnitPPELimits
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                organizationalUnitIds.Contains(
                    x.OrganizationalUnitId) &&
                ppeProductIds.Contains(
                    x.PPEProductId))
            .ToListAsync(
                cancellationToken);
    }

    public async Task AddAsync(
        OrganizationalUnitPPELimit limit,
        CancellationToken cancellationToken = default)
    {
        await _context
            .OrganizationalUnitPPELimits
            .AddAsync(
                limit,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }
}