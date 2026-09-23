using MediatR;

namespace PPEInventory.Application.Features.WarehouseProducts.Queries.GetByWarehouse;

public record GetWarehouseProductsByWarehouseQuery(
    int WarehouseId)
    : IRequest<IReadOnlyList<WarehouseProductDto>>;
