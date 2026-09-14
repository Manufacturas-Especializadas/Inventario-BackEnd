namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Update;

public record UpdatePurchaseOrderItemRequest(
    int PPEProductId,
    int OrderedPurchaseQuantity,
    decimal? PurchaseUnitCost);