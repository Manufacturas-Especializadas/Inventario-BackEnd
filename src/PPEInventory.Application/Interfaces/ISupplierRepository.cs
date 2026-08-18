using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface ISupplierRepository
{
    Task<IReadOnlyList<Supplier>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Supplier?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Supplier supplier,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}