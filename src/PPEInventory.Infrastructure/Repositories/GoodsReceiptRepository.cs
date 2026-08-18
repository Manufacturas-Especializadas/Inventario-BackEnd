using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class GoodsReceiptRepository
    : IGoodsReceiptRepository
{
    private readonly ApplicationDbContext _context;

    public GoodsReceiptRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsByPurchaseOrderIdAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken = default)
    {
        return _context.GoodsReceipts.AnyAsync(
            x => x.PurchaseOrderId == purchaseOrderId,
            cancellationToken);
    }

    public async Task AddAsync(
        GoodsReceipt goodsReceipt,
        CancellationToken cancellationToken = default)
    {
        await _context.GoodsReceipts.AddAsync(
            goodsReceipt,
            cancellationToken);
    }
}