using PPEInventory.Domain.Entities;

using PPEInventory.Application.Features.Employees;

namespace PPEInventory.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<EmployeeIdentityDto?> GetIdentityByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

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

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdForUpdateAsync(
    int id,
    CancellationToken cancellationToken = default);
}
