using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Features.Warehouses;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetReceivingWarehouses;

public class GetReceivingWarehousesQueryHandler
    : IRequestHandler<
        GetReceivingWarehousesQuery,
        IReadOnlyList<WarehouseDto>>
{
    private readonly IPurchaseOrderRepository
        _purchaseOrderRepository;

    private readonly IWarehouseProductRepository
        _warehouseProductRepository;

    public GetReceivingWarehousesQueryHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IWarehouseProductRepository warehouseProductRepository)
    {
        _purchaseOrderRepository =
            purchaseOrderRepository;

        _warehouseProductRepository =
            warehouseProductRepository;
    }

    public async Task<IReadOnlyList<WarehouseDto>> Handle(
        GetReceivingWarehousesQuery request,
        CancellationToken cancellationToken)
    {
        var folio =
            request.Folio
                .Trim()
                .ToUpperInvariant();

        var purchaseOrder =
            await _purchaseOrderRepository
                .GetByFolioAsync(
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

        if (purchaseOrder.Items.Count == 0)
        {
            throw new ConflictException(
                $"Purchase order '{folio}' does not contain items.");
        }

        var productIds =
            purchaseOrder.Items
                .Select(x =>
                    x.PPEProductId)
                .Distinct()
                .ToArray();

        var warehouses =
            await _warehouseProductRepository
                .GetWarehousesSupportingProductsAsync(
                    productIds,
                    cancellationToken);

        return warehouses
            .Select(warehouse =>
                new WarehouseDto
                {
                    Id = warehouse.Id,
                    Code = warehouse.Code,
                    Name = warehouse.Name,
                    Description =
                        warehouse.Description,
                    IsActive =
                        warehouse.IsActive,
                    CreatedAt =
                        warehouse.CreatedAt
                })
            .ToArray();
    }
}