namespace PPEInventory.Application.Features.PPEProducts.Commands.Update;

public class UpdatePPEProductRequest
{
    public int CategoryId { get; set; }

    public string Name { get; set; } =
        string.Empty;

    public string? Description { get; set; }

    public int? SizeId { get; set; }

    public int? ColorId { get; set; }

    public string? Model { get; set; }

    public string? Specification { get; set; }

    public int StockUnitId { get; set; }

    public int MinimumStock { get; set; }

    public int? DefaultMaxQuantityPerCycle
    {
        get;
        set;
    }

    public int? ReplacementIntervalDays
    {
        get;
        set;
    }
}