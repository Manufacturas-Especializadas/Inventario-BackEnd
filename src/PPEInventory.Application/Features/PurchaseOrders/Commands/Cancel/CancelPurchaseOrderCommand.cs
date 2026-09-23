using MediatR;

namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Cancel;

public record CancelPurchaseOrderCommand(
    string Folio,
    string Reason)
    : IRequest<PurchaseOrderDto>;