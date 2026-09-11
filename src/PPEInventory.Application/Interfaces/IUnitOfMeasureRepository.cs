using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IUnitOfMeasureRepository
{
    Task<IReadOnlyList<UnitOfMeasure>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<UnitOfMeasure?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<UnitOfMeasure?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, int excludeId, CancellationToken cancellationToken = default);

    Task AddAsync(UnitOfMeasure unit, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}