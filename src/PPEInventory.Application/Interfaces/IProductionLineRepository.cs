using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IProductionLineRepository
{
    Task<IReadOnlyList<ProductionLine>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductionLine?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int departmentId,
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductionLine productionLine,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}