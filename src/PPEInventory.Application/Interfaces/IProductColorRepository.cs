using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IProductColorRepository
{
    Task<IReadOnlyList<ProductColor>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductColor?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductColor?> GetByIdForUpdateAsync(
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
        ProductColor color,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}