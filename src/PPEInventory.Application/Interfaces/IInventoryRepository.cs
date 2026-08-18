using PPEInventory.Domain.Entities;
using PPEInventory.Application.Common.Models;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryBalance?> GetBalanceAsync(
        int warehouseId,
        int ppeProductId,
        CancellationToken cancellationToken = default);

    Task AddBalanceAsync(
        InventoryBalance balance,
        CancellationToken cancellationToken = default);

    Task AddMovementsAsync(
        IEnumerable<InventoryMovement> movements,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryBalance>> GetBalancesAsync(
        int? warehouseId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InventoryMovement>>
    GetMovementsAsync(
        int? warehouseId,
        int? ppeProductId,
        InventoryMovementType? movementType,
        DateTime? dateFrom,
        DateTime? dateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryBalance>>
    GetBalancesForUpdateAsync(
        int warehouseId,
        IReadOnlyCollection<int> productIds,
        CancellationToken cancellationToken = default);
}