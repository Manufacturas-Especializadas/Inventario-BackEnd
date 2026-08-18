using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Line)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Line)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Line)
            .FirstOrDefaultAsync(
                x => x.EmployeeNumber == employeeNumber,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(
                x => x.EmployeeNumber == employeeNumber,
                cancellationToken);
    }

    public async Task AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(
            employee,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}