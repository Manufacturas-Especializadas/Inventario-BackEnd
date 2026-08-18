using MediatR;

namespace PPEInventory.Application.Features.GoodsReceipts.Commands.ReceivePurchaseOrder;

public record ReceivePurchaseOrderCommand(
    string PurchaseOrderFolio,
    int WarehouseId,
    string? Notes)
    : IRequest<GoodsReceiptDto>;