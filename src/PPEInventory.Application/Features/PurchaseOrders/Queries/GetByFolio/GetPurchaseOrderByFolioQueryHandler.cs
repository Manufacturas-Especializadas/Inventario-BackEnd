using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetByFolio;

public class GetPurchaseOrderByFolioQueryHandler
    : IRequestHandler<
        GetPurchaseOrderByFolioQuery,
        PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;

    public GetPurchaseOrderByFolioQueryHandler(
        IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PurchaseOrderDto> Handle(
        GetPurchaseOrderByFolioQuery request,
        CancellationToken cancellationToken)
    {
        var folio =
            request.Folio.Trim().ToUpperInvariant();

        var po =
            await _repository.GetByFolioAsync(
                folio,
                cancellationToken);

        if (po is null)
        {
            throw new NotFoundException(
                $"Purchase order '{folio}' was not found.");
        }

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

            CurrencyCode = po.CurrencyCode,
            Notes = po.Notes,
            CreatedAt = po.CreatedAt,

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