using MediatR;

namespace PPEInventory.Application.Features.Inventory.Queries.GetBalances;

public record GetInventoryBalancesQuery(
    int? WarehouseId)
    : IRequest<IReadOnlyList<InventoryBalanceDto>>;