using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class UnitOfMeasureRepository
    : IUnitOfMeasureRepository
{
    private readonly ApplicationDbContext _context;

    public UnitOfMeasureRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UnitOfMeasure>>
        GetAllAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.Units
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<UnitOfMeasure?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Units
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<UnitOfMeasure?> GetByIdForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Units
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _context.Units
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        int excludeId,
        CancellationToken cancellationToken = default)
    {
        return _context.Units
            .AnyAsync(
                x =>
                    x.Id != excludeId &&
                    x.Name == name,
                cancellationToken);
    }

    public async Task AddAsync(
        UnitOfMeasure unit,
        CancellationToken cancellationToken = default)
    {
        await _context.Units.AddAsync(
            unit,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }
}