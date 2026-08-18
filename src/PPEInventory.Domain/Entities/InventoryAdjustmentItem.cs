namespace PPEInventory.Domain.Entities;

public class InventoryAdjustmentItem
{
    public int Id { get; set; }

    public int InventoryAdjustmentId { get; set; }

    public int PPEProductId { get; set; }

    public int QuantityAdjustment { get; set; }

    public int PreviousOnHandQuantity { get; set; }

    public int NewOnHandQuantity { get; set; }

    public int ReservedQuantitySnapshot { get; set; }

    public InventoryAdjustment InventoryAdjustment { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;
}