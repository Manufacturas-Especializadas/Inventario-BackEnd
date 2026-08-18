namespace PPEInventory.Domain.Entities;

public class InventoryBalance
{
    public int WarehouseId { get; set; }

    public int PPEProductId { get; set; }

    public int OnHandQuantity { get; set; }

    public int ReservedQuantity { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Warehouse Warehouse { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;

    public int AvailableQuantity =>
        OnHandQuantity - ReservedQuantity;
}