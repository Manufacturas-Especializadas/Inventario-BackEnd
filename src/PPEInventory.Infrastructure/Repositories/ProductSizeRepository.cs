using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class ProductSizeRepository
    : IProductSizeRepository
{
    private readonly ApplicationDbContext
        _context;

    public ProductSizeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<
        IReadOnlyList<ProductSize>>
        GetAllAsync(
            CancellationToken cancellationToken =
                default)
    {
        return await _context.ProductSizes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<ProductSize?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken =
            default)
    {
        return _context.ProductSizes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<ProductSize?>
        GetByIdForUpdateAsync(
            int id,
            CancellationToken cancellationToken =
                default)
    {
        return _context.ProductSizes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken =
            default)
    {
        return _context.ProductSizes.AnyAsync(
            x => x.Name == name,
            cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        int excludeId,
        CancellationToken cancellationToken =
            default)
    {
        return _context.ProductSizes.AnyAsync(
            x =>
                x.Name == name &&
                x.Id != excludeId,
            cancellationToken);
    }

    public async Task AddAsync(
        ProductSize size,
        CancellationToken cancellationToken =
            default)
    {
        await _context.ProductSizes.AddAsync(
            size,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken =
            default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }
}