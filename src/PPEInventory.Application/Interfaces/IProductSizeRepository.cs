using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IProductSizeRepository
{
    Task<IReadOnlyList<ProductSize>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductSize?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductSize?> GetByIdForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        int excludeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductSize size,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}