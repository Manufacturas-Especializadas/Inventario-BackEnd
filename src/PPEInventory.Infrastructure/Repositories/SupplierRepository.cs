using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;

    public SupplierRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Supplier?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _context.Suppliers
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public async Task AddAsync(
        Supplier supplier,
        CancellationToken cancellationToken = default)
    {
        await _context.Suppliers.AddAsync(
            supplier,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }
}