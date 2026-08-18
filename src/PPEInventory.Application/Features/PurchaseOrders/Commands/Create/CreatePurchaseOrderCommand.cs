using MediatR;

namespace PPEInventory.Application.Features.PurchaseOrders.Commands.Create;

public record CreatePurchaseOrderCommand(
    int SupplierId,
    string PurchaseOrderNumber,
    DateTime ConfirmedDeliveryDate,
    string CurrencyCode,
    string? Notes,
    IReadOnlyCollection<CreatePurchaseOrderItemRequest> Items)
    : IRequest<PurchaseOrderDto>;