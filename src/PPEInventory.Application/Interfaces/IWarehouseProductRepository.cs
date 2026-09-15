using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IWarehouseProductRepository
{
    Task<IReadOnlyList<WarehouseProduct>>
        GetByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WarehouseProduct>>
        GetActiveByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

    Task<WarehouseProduct?> GetAsync(
        int warehouseId,
        int ppeProductId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int warehouseId,
        int ppeProductId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        WarehouseProduct warehouseProduct,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}