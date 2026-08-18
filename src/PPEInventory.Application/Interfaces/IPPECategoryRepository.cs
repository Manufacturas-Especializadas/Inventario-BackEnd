using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IPPECategoryRepository
{
    Task<IReadOnlyList<PPECategory>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PPECategory?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PPECategory category,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}