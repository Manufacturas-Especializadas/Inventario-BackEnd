namespace PPEInventory.Domain.Entities;

public class InventoryCountItem
{
    public int Id { get; set; }

    public int InventoryCountId { get; set; }

    public int PPEProductId { get; set; }

    public int? CountedQuantity { get; set; }

    public int? SystemQuantitySnapshot { get; set; }

    public int? Variance { get; set; }

    public int? CountedByUserId { get; set; }

    public DateTime? CountedAt { get; set; }

    public InventoryCount InventoryCount { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;

    public User? CountedByUser { get; set; }
}