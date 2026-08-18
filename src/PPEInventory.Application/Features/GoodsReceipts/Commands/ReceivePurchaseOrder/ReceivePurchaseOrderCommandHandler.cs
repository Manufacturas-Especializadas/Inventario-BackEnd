using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.GoodsReceipts.Commands.ReceivePurchaseOrder;

public class ReceivePurchaseOrderCommandHandler
    : IRequestHandler<
        ReceivePurchaseOrderCommand,
        GoodsReceiptDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IGoodsReceiptRepository _goodsReceiptRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReceivePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IGoodsReceiptRepository goodsReceiptRepository,
        IWarehouseRepository warehouseRepository,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _goodsReceiptRepository = goodsReceiptRepository;
        _warehouseRepository = warehouseRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GoodsReceiptDto> Handle(
        ReceivePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var folio =
            request.PurchaseOrderFolio
                .Trim()
                .ToUpperInvariant();

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        await using var transaction =
            await _unitOfWork
                .BeginSerializableTransactionAsync(
                    cancellationToken);

        try
        {
            var purchaseOrder =
                await _purchaseOrderRepository
                    .GetByFolioForUpdateAsync(
                        folio,
                        cancellationToken);

            if (purchaseOrder is null)
            {
                throw new NotFoundException(
                    $"Purchase order '{folio}' was not found.");
            }

            if (purchaseOrder.Status !=
                PurchaseOrderStatus.Confirmed)
            {
                throw new ConflictException(
                    $"Purchase order '{folio}' cannot be received because its current status is '{purchaseOrder.Status}'.");
            }

            if (await _goodsReceiptRepository
                .ExistsByPurchaseOrderIdAsync(
                    purchaseOrder.Id,
                    cancellationToken))
            {
                throw new ConflictException(
                    $"Purchase order '{folio}' has already been received.");
            }

            var warehouse =
                await _warehouseRepository.GetByIdAsync(
                    request.WarehouseId,
                    cancellationToken);

            if (warehouse is null)
            {
                throw new NotFoundException(
                    $"Warehouse with id '{request.WarehouseId}' was not found.");
            }

            if (!warehouse.IsActive)
            {
                throw new ConflictException(
                    $"Warehouse '{warehouse.Name}' is inactive.");
            }

            if (purchaseOrder.Items.Count == 0)
            {
                throw new ConflictException(
                    $"Purchase order '{folio}' does not contain items.");
            }

            var now =
                _dateTimeProvider.UtcNow;

            var receipt =
                new GoodsReceipt
                {
                    PurchaseOrderId =
                        purchaseOrder.Id,

                    WarehouseId =
                        warehouse.Id,

                    ReceivedAt =
                        now,

                    ReceivedByUserId =
                        userId,

                    Notes =
                        Normalize(request.Notes),

                    CreatedAt =
                        now
                };

            foreach (var orderItem in
                purchaseOrder.Items
                    .OrderBy(x => x.PPEProductId))
            {
                var receivedQuantity =
                    checked(
                        orderItem.OrderedPurchaseQuantity *
                        orderItem.UnitsPerPackage);

                receipt.Items.Add(
                    new GoodsReceiptItem
                    {
                        PurchaseOrderItemId =
                            orderItem.Id,

                        PPEProductId =
                            orderItem.PPEProductId,

                        ReceivedQuantity =
                            receivedQuantity
                    });
            }

            await _goodsReceiptRepository.AddAsync(
                receipt,
                cancellationToken);

            purchaseOrder.Status =
                PurchaseOrderStatus.Received;

            purchaseOrder.UpdatedAt =
                now;

            purchaseOrder.UpdatedByUserId =
                userId;

            // Primer save:
            // genera Receipt.Id, Receipt.Folio e Item Ids.
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            var movements =
                new List<InventoryMovement>();

            foreach (var orderItem in
                purchaseOrder.Items
                    .OrderBy(x => x.PPEProductId))
            {
                var receivedQuantity =
                    checked(
                        orderItem.OrderedPurchaseQuantity *
                        orderItem.UnitsPerPackage);

                var balance =
                    await _inventoryRepository
                        .GetBalanceAsync(
                            warehouse.Id,
                            orderItem.PPEProductId,
                            cancellationToken);

                if (balance is null)
                {
                    balance =
                        new InventoryBalance
                        {
                            WarehouseId =
                                warehouse.Id,

                            PPEProductId =
                                orderItem.PPEProductId,

                            OnHandQuantity =
                                receivedQuantity,

                            ReservedQuantity =
                                0
                        };

                    await _inventoryRepository
                        .AddBalanceAsync(
                            balance,
                            cancellationToken);
                }
                else
                {
                    balance.OnHandQuantity +=
                        receivedQuantity;
                }

                movements.Add(
                    new InventoryMovement
                    {
                        WarehouseId =
                            warehouse.Id,

                        PPEProductId =
                            orderItem.PPEProductId,

                        MovementType =
                            InventoryMovementType
                                .PurchaseReceipt,

                        Quantity =
                            receivedQuantity,

                        ReferenceType =
                            InventoryReferenceType
                                .GoodsReceipt,

                        ReferenceId =
                            receipt.Id,

                        UnitCost =
                            CalculateStockUnitCost(
                                orderItem.PurchaseUnitCost,
                                orderItem.UnitsPerPackage),

                        Reason =
                            $"Purchase receipt {receipt.Folio}",

                        CreatedByUserId =
                            userId,

                        CreatedAt =
                            now
                    });
            }

            await _inventoryRepository
                .AddMovementsAsync(
                    movements,
                    cancellationToken);

            // Segundo save:
            // balances + inventory movements.
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new GoodsReceiptDto
            {
                Id = receipt.Id,
                Folio = receipt.Folio,

                PurchaseOrderId =
                    purchaseOrder.Id,

                PurchaseOrderFolio =
                    purchaseOrder.Folio,

                WarehouseId =
                    warehouse.Id,

                WarehouseName =
                    warehouse.Name,

                SupplierId =
                    purchaseOrder.SupplierId,

                SupplierName =
                    purchaseOrder.Supplier.Name,

                ReceivedAt =
                    receipt.ReceivedAt,

                Notes =
                    receipt.Notes,

                Items =
                    purchaseOrder.Items
                        .OrderBy(x => x.PPEProductId)
                        .Select(orderItem =>
                            new GoodsReceiptItemDto
                            {
                                PPEProductId =
                                    orderItem.PPEProductId,

                                Sku =
                                    orderItem.PPEProduct.Sku,

                                ProductName =
                                    orderItem.PPEProduct.Name,

                                PurchaseUnit =
                                    orderItem.PurchaseUnit,

                                UnitsPerPackage =
                                    orderItem.UnitsPerPackage,

                                OrderedPurchaseQuantity =
                                    orderItem.OrderedPurchaseQuantity,

                                ReceivedQuantity =
                                    checked(
                                        orderItem.OrderedPurchaseQuantity *
                                        orderItem.UnitsPerPackage)
                            })
                        .ToArray()
            };
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    private static decimal? CalculateStockUnitCost(
        decimal? purchaseUnitCost,
        int unitsPerPackage)
    {
        if (!purchaseUnitCost.HasValue)
        {
            return null;
        }

        return decimal.Round(
            purchaseUnitCost.Value /
            unitsPerPackage,
            4,
            MidpointRounding.AwayFromZero);
    }

    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}