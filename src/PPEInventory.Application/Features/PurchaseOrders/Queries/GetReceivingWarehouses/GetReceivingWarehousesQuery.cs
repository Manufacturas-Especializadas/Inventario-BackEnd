using MediatR;
using PPEInventory.Application.Features.Warehouses;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetReceivingWarehouses;

public record GetReceivingWarehousesQuery(
    string Folio)
    : IRequest<IReadOnlyList<WarehouseDto>>;