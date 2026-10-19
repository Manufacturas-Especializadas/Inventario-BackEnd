namespace PPEInventory.Application.Features.PPERequests;

public class RequestablePPEProductDto
{
    public int PPEProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Model { get; set; }
    public string? Specification { get; set; }
    public string StockUnit { get; set; } = string.Empty;
    public string? StockUnitSymbol { get; set; }
}
