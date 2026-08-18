using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class PurchaseOrderRepository
    : IPurchaseOrderRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseOrderRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsBySupplierAndNumberAsync(
        int supplierId,
        string purchaseOrderNumber,
        CancellationToken cancellationToken = default)
    {
        return _context.PurchaseOrders.AnyAsync(
            x =>
                x.SupplierId == supplierId &&
                x.PurchaseOrderNumber == purchaseOrderNumber,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .OrderByDescending(x => x.OrderDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<PurchaseOrder?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default)
    {
        return _context.PurchaseOrders
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }

    public async Task AddAsync(
        PurchaseOrder purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        await _context.PurchaseOrders.AddAsync(
            purchaseOrder,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }

    public Task<PurchaseOrder?> GetByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default)
    {
        return _context.PurchaseOrders
            .Include(x => x.Supplier)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }

}