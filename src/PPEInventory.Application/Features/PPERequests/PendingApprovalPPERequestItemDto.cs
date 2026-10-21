namespace PPEInventory.Application.Features.PPERequests;

public class PendingApprovalPPERequestItemDto
{
    public int PPEProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
