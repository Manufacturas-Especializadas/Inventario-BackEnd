namespace PPEInventory.Application.Features.GoodsReceipts;

public class GoodsReceiptDto
{
    public int Id { get; set; }

    public string Folio { get; set; } = string.Empty;

    public int PurchaseOrderId { get; set; }

    public string PurchaseOrderFolio { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public DateTime ReceivedAt { get; set; }

    public string? Notes { get; set; }

    public IReadOnlyCollection<GoodsReceiptItemDto> Items { get; set; }
        = Array.Empty<GoodsReceiptItemDto>();
}