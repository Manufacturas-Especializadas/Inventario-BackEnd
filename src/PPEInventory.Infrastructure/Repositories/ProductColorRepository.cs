using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class ProductColorRepository
    : IProductColorRepository
{
    private readonly ApplicationDbContext
        _context;

    public ProductColorRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<
        IReadOnlyList<ProductColor>>
        GetAllAsync(
            CancellationToken cancellationToken =
                default)
    {
        return await _context.ProductColors
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<ProductColor?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken =
            default)
    {
        return _context.ProductColors
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<ProductColor?>
        GetByIdForUpdateAsync(
            int id,
            CancellationToken cancellationToken =
                default)
    {
        return _context.ProductColors
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken =
            default)
    {
        return _context.ProductColors.AnyAsync(
            x => x.Name == name,
            cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        int excludeId,
        CancellationToken cancellationToken =
            default)
    {
        return _context.ProductColors.AnyAsync(
            x =>
                x.Name == name &&
                x.Id != excludeId,
            cancellationToken);
    }

    public async Task AddAsync(
        ProductColor color,
        CancellationToken cancellationToken =
            default)
    {
        await _context.ProductColors.AddAsync(
            color,
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