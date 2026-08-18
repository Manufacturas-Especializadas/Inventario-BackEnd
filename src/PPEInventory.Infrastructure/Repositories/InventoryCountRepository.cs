using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class InventoryCountRepository
    : IInventoryCountRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryCountRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> HasOpenCountAsync(
        int warehouseId,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryCounts.AnyAsync(
            x =>
                x.WarehouseId == warehouseId &&
                (
                    x.Status == InventoryCountStatus.Draft ||
                    x.Status == InventoryCountStatus.PendingReview
                ),
            cancellationToken);
    }

    public async Task AddAsync(
        InventoryCount inventoryCount,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryCounts.AddAsync(
            inventoryCount,
            cancellationToken);
    }

    public Task<InventoryCount?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryCounts
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
                    .ThenInclude(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }

    public Task<InventoryCount?> GetByFolioForUpdateAsync(
        string folio,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryCounts
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
                    .ThenInclude(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryCount>>
        GetPendingReviewAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.InventoryCounts
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
                    .ThenInclude(x => x.Category)
            .Where(x =>
                x.Status ==
                    InventoryCountStatus.PendingReview)
            .OrderBy(x => x.SubmittedAt)
            .ToListAsync(cancellationToken);
    }
}