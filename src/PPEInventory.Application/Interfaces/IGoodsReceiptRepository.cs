using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IGoodsReceiptRepository
{
    Task<bool> ExistsByPurchaseOrderIdAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        GoodsReceipt goodsReceipt,
        CancellationToken cancellationToken = default);
}