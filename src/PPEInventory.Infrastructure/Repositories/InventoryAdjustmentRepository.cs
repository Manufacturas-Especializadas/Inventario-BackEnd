using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class InventoryAdjustmentRepository
    : IInventoryAdjustmentRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryAdjustmentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        InventoryAdjustment adjustment,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryAdjustments.AddAsync(
            adjustment,
            cancellationToken);
    }

    public Task<InventoryAdjustment?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryAdjustments
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.CreatedByUser)
                .ThenInclude(x => x.Employee)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }
}