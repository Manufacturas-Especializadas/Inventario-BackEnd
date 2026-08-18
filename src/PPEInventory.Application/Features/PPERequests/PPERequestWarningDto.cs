namespace PPEInventory.Application.Features.PPERequests;

public class PPERequestWarningDto
{
    public string Code { get; set; } =
        "EARLY_REPLACEMENT";

    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public DateTime LastDeliveredAt { get; set; }

    public DateTime NextEligibleDate { get; set; }

    public string Message { get; set; } = string.Empty;
}