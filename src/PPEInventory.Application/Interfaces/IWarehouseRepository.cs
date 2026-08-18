using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IWarehouseRepository
{
    Task<IReadOnlyList<Warehouse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Warehouse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}