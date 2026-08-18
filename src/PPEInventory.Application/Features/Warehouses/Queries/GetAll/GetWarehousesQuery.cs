using MediatR;

namespace PPEInventory.Application.Features.Warehouses.Queries.GetAll;

public record GetWarehousesQuery
    : IRequest<IReadOnlyList<WarehouseDto>>;