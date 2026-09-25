using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class WarehouseProductRepository
    : IWarehouseProductRepository
{
    private readonly ApplicationDbContext _context;

    public WarehouseProductRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IReadOnlyList<WarehouseProduct>>
        GetByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default)
    {
        return await _context.WarehouseProducts
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.PPEProduct)
                .ThenInclude(x => x.StockUnitOfMeasure)
            .Where(x =>
                x.WarehouseId == warehouseId)
            .OrderBy(x => x.PPEProduct.Name)
            .ToListAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<WarehouseProduct>>
        GetActiveByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default)
    {
        return await _context.WarehouseProducts
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.PPEProduct)
                .ThenInclude(x => x.Category)
            .Where(x =>
                x.WarehouseId == warehouseId &&
                x.IsActive &&
                x.PPEProduct.IsActive)
            .OrderBy(x => x.PPEProduct.Name)
            .ToListAsync(cancellationToken);
    }


    public Task<WarehouseProduct?> GetAsync(
        int warehouseId,
        int ppeProductId,
        CancellationToken cancellationToken = default)
    {
        return _context.WarehouseProducts
            .Include(x => x.Warehouse)
            .Include(x => x.PPEProduct)
                .ThenInclude(x => x.StockUnitOfMeasure)
            .FirstOrDefaultAsync(
                x =>
                    x.WarehouseId == warehouseId &&
                    x.PPEProductId == ppeProductId,
                cancellationToken);
    }


    public Task<bool> ExistsAsync(
        int warehouseId,
        int ppeProductId,
        CancellationToken cancellationToken = default)
    {
        return _context.WarehouseProducts
            .AnyAsync(
                x =>
                    x.WarehouseId == warehouseId &&
                    x.PPEProductId == ppeProductId,
                cancellationToken);
    }


    public async Task AddAsync(
        WarehouseProduct warehouseProduct,
        CancellationToken cancellationToken = default)
    {
        await _context.WarehouseProducts.AddAsync(
            warehouseProduct,
            cancellationToken);
    }


    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyList<Warehouse>>
    GetWarehousesSupportingProductsAsync(
        IReadOnlyCollection<int> ppeProductIds,
        CancellationToken cancellationToken = default)
    {
        var productIds =
            ppeProductIds
                .Distinct()
                .ToArray();

        if (productIds.Length == 0)
        {
            return Array.Empty<Warehouse>();
        }

        var warehouseIds =
            await _context.WarehouseProducts
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Warehouse.IsActive &&
                    x.PPEProduct.IsActive &&
                    productIds.Contains(
                        x.PPEProductId))
                .GroupBy(x =>
                    x.WarehouseId)
                .Where(group =>
                    group
                        .Select(x =>
                            x.PPEProductId)
                        .Distinct()
                        .Count() ==
                    productIds.Length)
                .Select(group =>
                    group.Key)
                .ToListAsync(
                    cancellationToken);

        return await _context.Warehouses
            .AsNoTracking()
            .Where(x =>
                warehouseIds.Contains(x.Id))
            .OrderBy(x => x.Name)
            .ToListAsync(
                cancellationToken);
    }

    public async Task<IReadOnlyList<WarehouseProduct>>
    GetByWarehousesAndProductsAsync(
        IReadOnlyCollection<int> warehouseIds,
        IReadOnlyCollection<int> ppeProductIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.WarehouseProducts
            .Include(x => x.Warehouse)
            .Include(x => x.PPEProduct)
            .Where(x =>
                warehouseIds.Contains(x.WarehouseId) &&
                ppeProductIds.Contains(x.PPEProductId))
            .ToListAsync(cancellationToken);
    }

    public Task<WarehouseProduct?> GetForUpdateAsync(
    int warehouseId,
    int ppeProductId,
    CancellationToken cancellationToken = default)
    {
        return _context.WarehouseProducts
            .FromSqlInterpolated($"""
            SELECT *
            FROM WarehouseProducts WITH (UPDLOCK, HOLDLOCK)
            WHERE WarehouseId = {warehouseId}
              AND PPEProductId = {ppeProductId}
            """)
            .Include(x => x.Warehouse)
            .Include(x => x.PPEProduct)
                .ThenInclude(x => x.StockUnitOfMeasure)
            .FirstOrDefaultAsync(
                cancellationToken);
    }

}