using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class PPEProductRepository
    : IPPEProductRepository
{
    private readonly ApplicationDbContext _context;

    public PPEProductRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PPEProduct>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PPEProducts
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Sku)
            .ToListAsync(cancellationToken);
    }

    public Task<PPEProduct?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.PPEProducts
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        PPEProduct product,
        CancellationToken cancellationToken = default)
    {
        await _context.PPEProducts.AddAsync(
            product,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyList<PPEProduct>> GetByIdsAsync(
    IReadOnlyCollection<int> ids,
    CancellationToken cancellationToken = default)
    {
        return await _context.PPEProducts
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

}