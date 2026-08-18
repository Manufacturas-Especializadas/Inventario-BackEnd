using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PurchaseOrders.Commands.Create;

public class CreatePurchaseOrderCommandHandler
    : IRequestHandler<
        CreatePurchaseOrderCommand,
        PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IProductSupplierRepository _productSupplierRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        ISupplierRepository supplierRepository,
        IProductSupplierRepository productSupplierRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _supplierRepository = supplierRepository;
        _productSupplierRepository = productSupplierRepository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PurchaseOrderDto> Handle(
        CreatePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var supplier =
            await _supplierRepository.GetByIdAsync(
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

        if (await _purchaseOrderRepository
            .ExistsBySupplierAndNumberAsync(
                supplier.Id,
                purchaseOrderNumber,
                cancellationToken))
        {
            throw new ConflictException(
                $"Purchase order number '{purchaseOrderNumber}' already exists for supplier '{supplier.Name}'.");
        }

        var productIds = request.Items
            .Select(x => x.PPEProductId)
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
                    !relationsByProductId.ContainsKey(id))
                .ToArray();

        if (missingProductIds.Length > 0)
        {
            throw new ConflictException(
                $"The following PPE product(s) are not configured as active products for supplier '{supplier.Name}': {string.Join(", ", missingProductIds)}.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var now = _dateTimeProvider.UtcNow;

        var purchaseOrder = new PurchaseOrder
        {
            SupplierId = supplier.Id,

            PurchaseOrderNumber =
                purchaseOrderNumber,

            Status =
                PurchaseOrderStatus.Confirmed,

            OrderDate =
                now.Date,

            ConfirmedDeliveryDate =
                request.ConfirmedDeliveryDate.Date,

            SupplierConfirmedAt =
                now,

            CurrencyCode =
                request.CurrencyCode
                    .Trim()
                    .ToUpperInvariant(),

            Notes =
                Normalize(request.Notes),

            CreatedAt =
                now,

            CreatedByUserId =
                userId
        };

        foreach (var requestItem in request.Items)
        {
            var productSupplier =
                relationsByProductId[
                    requestItem.PPEProductId];

            purchaseOrder.Items.Add(
                new PurchaseOrderItem
                {
                    PPEProductId =
                        productSupplier.PPEProductId,

                    SupplierProductCode =
                        productSupplier.SupplierProductCode,

                    PurchaseUnit =
                        productSupplier.PurchaseUnit,

                    UnitsPerPackage =
                        productSupplier.UnitsPerPackage,

                    OrderedPurchaseQuantity =
                        requestItem.OrderedPurchaseQuantity,

                    PurchaseUnitCost =
                        requestItem.PurchaseUnitCost
                });
        }

        await _purchaseOrderRepository.AddAsync(
            purchaseOrder,
            cancellationToken);

        await _purchaseOrderRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(
            purchaseOrder,
            supplier.Name,
            relationsByProductId);
    }

    private static PurchaseOrderDto MapToDto(
        PurchaseOrder purchaseOrder,
        string supplierName,
        IReadOnlyDictionary<int, ProductSupplier>
            productSuppliers)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            Folio = purchaseOrder.Folio,

            SupplierId = purchaseOrder.SupplierId,
            SupplierName = supplierName,

            PurchaseOrderNumber =
                purchaseOrder.PurchaseOrderNumber,

            Status =
                purchaseOrder.Status,

            OrderDate =
                purchaseOrder.OrderDate,

            ConfirmedDeliveryDate =
                purchaseOrder.ConfirmedDeliveryDate,

            SupplierConfirmedAt =
                purchaseOrder.SupplierConfirmedAt,

            CurrencyCode =
                purchaseOrder.CurrencyCode,

            Notes =
                purchaseOrder.Notes,

            CreatedAt =
                purchaseOrder.CreatedAt,

            Items = purchaseOrder.Items
                .Select(item =>
                {
                    var relation =
                        productSuppliers[
                            item.PPEProductId];

                    return new PurchaseOrderItemDto
                    {
                        Id = item.Id,

                        PPEProductId =
                            item.PPEProductId,

                        Sku =
                            relation.PPEProduct.Sku,

                        ProductName =
                            relation.PPEProduct.Name,

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
                    };
                })
                .ToArray()
        };
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}