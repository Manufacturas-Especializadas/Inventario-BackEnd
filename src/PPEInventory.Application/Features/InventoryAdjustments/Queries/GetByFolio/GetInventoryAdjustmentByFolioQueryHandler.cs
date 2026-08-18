using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.InventoryAdjustments.Queries.GetByFolio;

public class GetInventoryAdjustmentByFolioQueryHandler
    : IRequestHandler<
        GetInventoryAdjustmentByFolioQuery,
        InventoryAdjustmentDto>
{
    private readonly IInventoryAdjustmentRepository _repository;

    public GetInventoryAdjustmentByFolioQueryHandler(
        IInventoryAdjustmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryAdjustmentDto> Handle(
        GetInventoryAdjustmentByFolioQuery request,
        CancellationToken cancellationToken)
    {
        var folio =
            request.Folio
                .Trim()
                .ToUpperInvariant();

        var adjustment =
            await _repository.GetByFolioAsync(
                folio,
                cancellationToken);

        if (adjustment is null)
        {
            throw new NotFoundException(
                $"Inventory adjustment '{folio}' was not found.");
        }

        return new InventoryAdjustmentDto
        {
            Id =
                adjustment.Id,

            Folio =
                adjustment.Folio,

            WarehouseId =
                adjustment.WarehouseId,

            WarehouseCode =
                adjustment.Warehouse.Code,

            WarehouseName =
                adjustment.Warehouse.Name,

            Reason =
                adjustment.Reason,

            CreatedByUserId =
                adjustment.CreatedByUserId,

            CreatedAt =
                adjustment.CreatedAt,

            Items =
                adjustment.Items
                    .Select(item =>
                        new InventoryAdjustmentItemDto
                        {
                            PPEProductId =
                                item.PPEProductId,

                            Sku =
                                item.PPEProduct.Sku,

                            ProductName =
                                item.PPEProduct.Name,

                            QuantityAdjustment =
                                item.QuantityAdjustment,

                            PreviousOnHandQuantity =
                                item.PreviousOnHandQuantity,

                            NewOnHandQuantity =
                                item.NewOnHandQuantity,

                            // Este dato es histórico en términos
                            // de OnHand; Reserved no está almacenado
                            // en el documento.
                            ReservedQuantity =
                                item.ReservedQuantitySnapshot,

                            AvailableQuantity =
                                item.NewOnHandQuantity -
                                item.ReservedQuantitySnapshot
                        })
                    .ToArray()
        };
    }
}