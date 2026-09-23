namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Update;

public class UpdatePurchaseOrderRequest
{
    public int SupplierId { get; set; }

    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public DateTime ConfirmedDeliveryDate { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public IReadOnlyCollection<UpdatePurchaseOrderItemRequest> Items { get; set; } 
        = Array.Empty<UpdatePurchaseOrderItemRequest>();
}