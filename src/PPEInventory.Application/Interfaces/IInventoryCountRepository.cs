using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IInventoryCountRepository
{
    Task<bool> HasOpenCountAsync(
        int warehouseId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InventoryCount inventoryCount,
        CancellationToken cancellationToken = default);

    Task<InventoryCount?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default);

    Task<InventoryCount?> GetByFolioForUpdateAsync(
        string folio,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryCount>> GetPendingReviewAsync(
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryCount>>
    GetDraftCountsAsync(
        CancellationToken cancellationToken = default);
    Task<bool> HasDraftCountAsync(
    int warehouseId,
    CancellationToken cancellationToken = default);

    void Remove(InventoryCount inventoryCount);

    Task<InventoryCount?> GetHeaderByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default);

    Task<InventoryCountItem?> GetItemAsync(
        int inventoryCountId,
        int ppeProductId,
        CancellationToken cancellationToken = default);

}