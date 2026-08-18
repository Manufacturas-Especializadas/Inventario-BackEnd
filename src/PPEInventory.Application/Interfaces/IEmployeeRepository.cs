using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}