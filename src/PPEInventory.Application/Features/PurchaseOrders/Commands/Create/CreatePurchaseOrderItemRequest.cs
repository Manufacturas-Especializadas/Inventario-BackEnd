namespace PPEInventory.Application.Features.PurchaseOrders.Commands.Create;

public record CreatePurchaseOrderItemRequest(
    int PPEProductId,
    int OrderedPurchaseQuantity,
    decimal? PurchaseUnitCost);