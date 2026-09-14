using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Update;

public class UpdatePurchaseOrderCommandHandler
    : IRequestHandler<
        UpdatePurchaseOrderCommand,
        PurchaseOrderDto>
{
    private readonly
        IPurchaseOrderRepository
        _purchaseOrderRepository;

    private readonly
        ISupplierRepository
        _supplierRepository;

    private readonly
        IProductSupplierRepository
        _productSupplierRepository;

    private readonly
        ICurrentUserService
        _currentUser;

    private readonly
        IDateTimeProvider
        _dateTimeProvider;

    public UpdatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository
            purchaseOrderRepository,
        ISupplierRepository
            supplierRepository,
        IProductSupplierRepository
            productSupplierRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _purchaseOrderRepository =
            purchaseOrderRepository;

        _supplierRepository =
            supplierRepository;

        _productSupplierRepository =
            productSupplierRepository;

        _currentUser =
            currentUser;

        _dateTimeProvider =
            dateTimeProvider;
    }

    public async Task<PurchaseOrderDto>
        Handle(
            UpdatePurchaseOrderCommand request,
            CancellationToken cancellationToken)
    {
        var folio =
            request.Folio
                .Trim()
                .ToUpperInvariant();

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

        if (purchaseOrder.Status ==
            PurchaseOrderStatus.Received)
        {
            throw new ConflictException(
                "A received purchase order cannot be edited.");
        }

        if (purchaseOrder.Status ==
            PurchaseOrderStatus.Cancelled)
        {
            throw new ConflictException(
                "A cancelled purchase order cannot be edited.");
        }

        var supplier =
            await _supplierRepository
                .GetByIdAsync(
                    request.SupplierId,
                    cancellationToken);

        if (supplier is null)
        {
            throw new NotFoundException(
                $"Supplier with id '{request.SupplierId}' was not found.");
        }

        if (!supplier.IsActive)
        {
            throw new ConflictException(
                $"Supplier '{supplier.Name}' is inactive.");
        }

        var purchaseOrderNumber =
            request.PurchaseOrderNumber.Trim();

        var duplicate =
            await _purchaseOrderRepository
                .ExistsBySupplierAndNumberAsync(
                    supplier.Id,
                    purchaseOrderNumber,
                    purchaseOrder.Id,
                    cancellationToken);

        if (duplicate)
        {
            throw new ConflictException(
                $"Purchase order number '{purchaseOrderNumber}' already exists for supplier '{supplier.Name}'.");
        }

        var productIds =
            request.Items
                .Select(x =>
                    x.PPEProductId)
                .Distinct()
                .ToArray();

        var productSuppliers =
            await _productSupplierRepository
                .GetBySupplierAndProductIdsAsync(
                    supplier.Id,
                    productIds,
                    cancellationToken);

        var relationsByProductId =
            productSuppliers.ToDictionary(
                x => x.PPEProductId);

        var missingProductIds =
            productIds
                .Where(id =>
                    !relationsByProductId
                        .ContainsKey(id))
                .ToArray();

        if (missingProductIds.Length > 0)
        {
            throw new ConflictException(
                $"The following PPE product(s) are not configured as active products for supplier '{supplier.Name}': {string.Join(", ", missingProductIds)}.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        purchaseOrder.SupplierId =
            supplier.Id;

        purchaseOrder.PurchaseOrderNumber =
            purchaseOrderNumber;

        purchaseOrder.ConfirmedDeliveryDate =
            request.ConfirmedDeliveryDate.Date;

        purchaseOrder.CurrencyCode =
            request.CurrencyCode
                .Trim()
                .ToUpperInvariant();

        purchaseOrder.Notes =
            Normalize(request.Notes);

        purchaseOrder.UpdatedAt =
            _dateTimeProvider.UtcNow;

        purchaseOrder.UpdatedByUserId =
            userId;

        /*
         * La relación PurchaseOrder -> Items
         * está configurada con Cascade.
         * Al quitar los hijos requeridos EF Core
         * los marca para eliminación.
         */
        purchaseOrder.Items.Clear();

        foreach (
            var requestItem in
            request.Items)
        {
            var relation =
                relationsByProductId[
                    requestItem.PPEProductId];

            purchaseOrder.Items.Add(
                new PurchaseOrderItem
                {
                    PPEProductId =
                        relation.PPEProductId,

                    SupplierProductCode =
                        relation.SupplierProductCode,

                    PurchaseUnit =
                        relation
                            .PurchaseUnitOfMeasure
                            .Name,

                    UnitsPerPackage =
                        relation.UnitsPerPackage,

                    OrderedPurchaseQuantity =
                        requestItem
                            .OrderedPurchaseQuantity,

                    PurchaseUnitCost =
                        requestItem
                            .PurchaseUnitCost,

                    PPEProduct =
                        relation.PPEProduct
                });
        }

        await _purchaseOrderRepository
            .SaveChangesAsync(
                cancellationToken);

        return Map(
            purchaseOrder,
            supplier.Name);
    }

    private static PurchaseOrderDto Map(
        PurchaseOrder po,
        string supplierName)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            Folio = po.Folio,

            SupplierId = po.SupplierId,
            SupplierName = supplierName,

            PurchaseOrderNumber =
                po.PurchaseOrderNumber,

            Status = po.Status,
            OrderDate = po.OrderDate,

            ConfirmedDeliveryDate =
                po.ConfirmedDeliveryDate,

            SupplierConfirmedAt =
                po.SupplierConfirmedAt,

            CurrencyCode =
                po.CurrencyCode,

            Notes = po.Notes,

            CreatedAt = po.CreatedAt,
            UpdatedAt = po.UpdatedAt,

            CancelledAt = po.CancelledAt,

            CancellationReason =
                po.CancellationReason,

            Items = po.Items
                .Select(item =>
                    new PurchaseOrderItemDto
                    {
                        Id = item.Id,

                        PPEProductId =
                            item.PPEProductId,

                        Sku =
                            item.PPEProduct.Sku,

                        ProductName =
                            item.PPEProduct.Name,

                        SupplierProductCode =
                            item.SupplierProductCode,

                        PurchaseUnit =
                            item.PurchaseUnit,

                        UnitsPerPackage =
                            item.UnitsPerPackage,

                        OrderedPurchaseQuantity =
                            item.OrderedPurchaseQuantity,

                        PurchaseUnitCost =
                            item.PurchaseUnitCost
                    })
                .ToArray()
        };
    }

    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
            value)
            ? null
            : value.Trim();
    }
}