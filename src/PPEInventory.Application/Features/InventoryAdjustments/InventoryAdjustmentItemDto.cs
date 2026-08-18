namespace PPEInventory.Application.Features.InventoryAdjustments;

public class InventoryAdjustmentItemDto
{
    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int QuantityAdjustment { get; set; }

    public int PreviousOnHandQuantity { get; set; }

    public int NewOnHandQuantity { get; set; }

    public int ReservedQuantity { get; set; }

    public int AvailableQuantity { get; set; }
}