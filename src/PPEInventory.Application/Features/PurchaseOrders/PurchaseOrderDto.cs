using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PurchaseOrders;

public class PurchaseOrderDto
{
    public int Id { get; set; }

    public string Folio { get; set; } = string.Empty;

    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public PurchaseOrderStatus Status { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ConfirmedDeliveryDate { get; set; }

    public DateTime? SupplierConfirmedAt { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public IReadOnlyCollection<PurchaseOrderItemDto> Items { get; set; }
        = Array.Empty<PurchaseOrderItemDto>();
}