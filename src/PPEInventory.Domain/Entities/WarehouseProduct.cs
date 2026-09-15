namespace PPEInventory.Domain.Entities;

public class WarehouseProduct
{
    public int WarehouseId { get; set; }

    public int PPEProductId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }


    public Warehouse Warehouse { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;

    public User? CreatedByUser { get; set; }

    public User? UpdatedByUser { get; set; }
}