using MediatR;

namespace PPEInventory.Application.Features.Inventory.Queries.GetLowStock;

public record GetLowStockQuery(
    int WarehouseId)
    : IRequest<IReadOnlyList<LowStockDto>>;