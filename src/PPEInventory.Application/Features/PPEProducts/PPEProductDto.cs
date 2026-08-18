namespace PPEInventory.Application.Features.PPEProducts;

public class PPEProductDto
{
    public int Id { get; set; }

    public string Sku { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public string? Model { get; set; }

    public string? Specification { get; set; }

    public string StockUnit { get; set; } = string.Empty;

    public int MinimumStock { get; set; }

    public int? MaxQuantityPerRequest { get; set; }

    public int? ReplacementIntervalDays { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}