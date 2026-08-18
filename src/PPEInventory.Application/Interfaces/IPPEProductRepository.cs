using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IPPEProductRepository
{
    Task<IReadOnlyList<PPEProduct>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PPEProduct?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PPEProduct product,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PPEProduct>> GetByIdsAsync(
    IReadOnlyCollection<int> ids,
    CancellationToken cancellationToken = default);

}