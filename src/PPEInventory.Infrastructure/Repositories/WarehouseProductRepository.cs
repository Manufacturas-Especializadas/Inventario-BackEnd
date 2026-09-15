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
}