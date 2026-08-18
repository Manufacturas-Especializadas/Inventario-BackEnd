using MediatR;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Inventory.Queries.GetMovements;

public class GetInventoryMovementsQueryHandler
    : IRequestHandler<
        GetInventoryMovementsQuery,
        PagedResult<InventoryMovementDto>>
{
    private readonly IInventoryRepository _repository;

    public GetInventoryMovementsQueryHandler(
        IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<InventoryMovementDto>> Handle(
        GetInventoryMovementsQuery request,
        CancellationToken cancellationToken)
    {
        var result =
            await _repository.GetMovementsAsync(
                request.WarehouseId,
                request.PPEProductId,
                request.MovementType,
                request.DateFrom,
                request.DateTo,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        return new PagedResult<InventoryMovementDto>
        {
            PageNumber =
                result.PageNumber,

            PageSize =
                result.PageSize,

            TotalCount =
                result.TotalCount,

            Items =
                result.Items
                    .Select(x =>
                        new InventoryMovementDto
                        {
                            Id = x.Id,

                            WarehouseId =
                                x.WarehouseId,

                            WarehouseCode =
                                x.Warehouse.Code,

                            PPEProductId =
                                x.PPEProductId,

                            Sku =
                                x.PPEProduct.Sku,

                            ProductName =
                                x.PPEProduct.Name,

                            MovementType =
                                x.MovementType,

                            Quantity =
                                x.Quantity,

                            ReferenceType =
                                x.ReferenceType,

                            ReferenceId =
                                x.ReferenceId,

                            UnitCost =
                                x.UnitCost,

                            Reason =
                                x.Reason,

                            CreatedAt =
                                x.CreatedAt
                        })
                    .ToArray()
        };
    }
}