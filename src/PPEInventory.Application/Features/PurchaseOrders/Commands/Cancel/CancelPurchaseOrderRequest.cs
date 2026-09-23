namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Cancel;

public class CancelPurchaseOrderRequest
{
    public string Reason { get; set; }
        = string.Empty;
}