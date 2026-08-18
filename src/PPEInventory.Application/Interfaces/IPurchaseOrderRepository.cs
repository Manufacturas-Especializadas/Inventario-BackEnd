using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<bool> ExistsBySupplierAndNumberAsync(
        int supplierId,
        string purchaseOrderNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PurchaseOrder?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PurchaseOrder purchaseOrder,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<PurchaseOrder?> GetByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default);
}