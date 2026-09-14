using MediatR;

namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Update;

public record UpdatePurchaseOrderCommand(
    string Folio,
    int SupplierId,
    string PurchaseOrderNumber,
    DateTime ConfirmedDeliveryDate,
    string CurrencyCode,
    string? Notes,
    IReadOnlyCollection<
        UpdatePurchaseOrderItemRequest> Items)
    : IRequest<PurchaseOrderDto>;