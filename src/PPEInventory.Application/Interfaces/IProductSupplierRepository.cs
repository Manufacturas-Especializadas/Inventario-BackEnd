using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IProductSupplierRepository
{
    Task<bool> ExistsAsync(
        int ppeProductId,
        int supplierId,
        CancellationToken cancellationToken = default);

    Task<bool> HasPreferredSupplierAsync(
        int ppeProductId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductSupplier>>
        GetByProductIdAsync(
            int ppeProductId,
            CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductSupplier productSupplier,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductSupplier>>
    GetBySupplierAndProductIdsAsync(
        int supplierId,
        IReadOnlyCollection<int> productIds,
        CancellationToken cancellationToken = default);
}