namespace PPEInventory.Application.Features.PPERequests.Cancellation;

public class CancelledPPEItemDto
{
    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int ReleasedQuantity { get; set; }

    public int OnHandQuantity { get; set; }

    public int ReservedQuantity { get; set; }

    public int AvailableQuantity { get; set; }
}