namespace PPEInventory.Application.Features.InventoryCounts;

public class InventoryCountItemDto
{
    public int Id { get; set; }

    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int? CountedQuantity { get; set; }

    public int? SystemQuantity { get; set; }

    public int? Variance { get; set; }

    public DateTime? CountedAt { get; set; }
}