using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetAll;

public class GetPurchaseOrdersQueryHandler
    : IRequestHandler<
        GetPurchaseOrdersQuery,
        IReadOnlyList<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;

    public GetPurchaseOrdersQueryHandler(
        IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PurchaseOrderDto>> Handle(
        GetPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var purchaseOrders =
            await _repository.GetAllAsync(
                cancellationToken);

        return purchaseOrders
            .Select(Map)
            .ToArray();
    }

    private static PurchaseOrderDto Map(
        Domain.Entities.PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            Folio = po.Folio,

            SupplierId = po.SupplierId,
            SupplierName = po.Supplier.Name,

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

            Notes =
                po.Notes,

            CreatedAt =
                po.CreatedAt,

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
}