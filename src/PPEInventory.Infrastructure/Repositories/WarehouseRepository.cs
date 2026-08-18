using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class WarehouseRepository
    : IWarehouseRepository
{
    private readonly ApplicationDbContext _context;

    public WarehouseRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Warehouse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Warehouse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _context.Warehouses
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task AddAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default)
    {
        await _context.Warehouses.AddAsync(
            warehouse,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }
}