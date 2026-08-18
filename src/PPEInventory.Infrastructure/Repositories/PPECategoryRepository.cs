using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class PPECategoryRepository
    : IPPECategoryRepository
{
    private readonly ApplicationDbContext _context;

    public PPECategoryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PPECategory>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PPECategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<PPECategory?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.PPECategories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _context.PPECategories
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public async Task AddAsync(
        PPECategory category,
        CancellationToken cancellationToken = default)
    {
        await _context.PPECategories.AddAsync(
            category,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }
}