using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class ProductSupplierRepository
    : IProductSupplierRepository
{
    private readonly ApplicationDbContext _context;

    public ProductSupplierRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(
        int ppeProductId,
        int supplierId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductSuppliers.AnyAsync(
            x =>
                x.PPEProductId == ppeProductId &&
                x.SupplierId == supplierId,
            cancellationToken);
    }

    public Task<bool> HasPreferredSupplierAsync(
        int ppeProductId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductSuppliers.AnyAsync(
            x =>
                x.PPEProductId == ppeProductId &&
                x.IsPreferred &&
                x.IsActive,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ProductSupplier>>
        GetByProductIdAsync(
            int ppeProductId,
            CancellationToken cancellationToken = default)
    {
        return await _context.ProductSuppliers
            .AsNoTracking()
            .Include(x => x.PPEProduct)
            .Include(x => x.Supplier)
            .Where(x => x.PPEProductId == ppeProductId)
            .OrderByDescending(x => x.IsPreferred)
            .ThenBy(x => x.Supplier.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        ProductSupplier productSupplier,
        CancellationToken cancellationToken = default)
    {
        await _context.ProductSuppliers.AddAsync(
            productSupplier,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyList<ProductSupplier>>
    GetBySupplierAndProductIdsAsync(
        int supplierId,
        IReadOnlyCollection<int> productIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductSuppliers
            .AsNoTracking()
            .Include(x => x.PPEProduct)
            .Where(x =>
                x.SupplierId == supplierId &&
                productIds.Contains(x.PPEProductId) &&
                x.IsActive &&
                x.PPEProduct.IsActive)
            .ToListAsync(cancellationToken);
    }
}