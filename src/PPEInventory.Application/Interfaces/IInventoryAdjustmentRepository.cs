using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IInventoryAdjustmentRepository
{
    Task AddAsync(
        InventoryAdjustment adjustment,
        CancellationToken cancellationToken = default);

    Task<InventoryAdjustment?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryAdjustment>>
    GetFilteredAsync(
        int? warehouseId,
        DateTime? createdFrom,
        DateTime? createdToExclusive,
        CancellationToken cancellationToken = default);
}