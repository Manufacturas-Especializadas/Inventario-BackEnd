namespace PPEInventory.Application.Features.PPERequests.Delivery;

public class DeliveredPPEItemDto
{
    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int DeliveredQuantity { get; set; }

    public int OnHandQuantity { get; set; }

    public int ReservedQuantity { get; set; }

    public int AvailableQuantity { get; set; }
}