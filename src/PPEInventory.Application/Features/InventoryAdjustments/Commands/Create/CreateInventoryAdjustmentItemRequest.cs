namespace PPEInventory.Application.Features.InventoryAdjustments.Commands.Create;

public record CreateInventoryAdjustmentItemRequest(
    int PPEProductId,
    int QuantityAdjustment);