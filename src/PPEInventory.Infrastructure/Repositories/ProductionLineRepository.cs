using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class ProductionLineRepository : IProductionLineRepository
{
    private readonly ApplicationDbContext _context;

    public ProductionLineRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductionLine>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductionLines
            .AsNoTracking()
            .Include(x => x.Department)
            .OrderBy(x => x.Department.Name)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductionLine?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductionLines
            .AsNoTracking()
            .Include(x => x.Department)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        int departmentId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductionLines
            .AnyAsync(
                x =>
                    x.DepartmentId == departmentId &&
                    x.Name == name,
                cancellationToken);
    }

    public async Task AddAsync(
        ProductionLine productionLine,
        CancellationToken cancellationToken = default)
    {
        await _context.ProductionLines.AddAsync(
            productionLine,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}