using MediatR;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetAll;

public record GetPurchaseOrdersQuery
    : IRequest<IReadOnlyList<PurchaseOrderDto>>;