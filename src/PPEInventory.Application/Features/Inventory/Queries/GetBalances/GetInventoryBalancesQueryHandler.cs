using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Inventory.Queries.GetBalances;

public class GetInventoryBalancesQueryHandler
    : IRequestHandler<
        GetInventoryBalancesQuery,
        IReadOnlyList<InventoryBalanceDto>>
{
    private readonly IInventoryRepository _repository;

    public GetInventoryBalancesQueryHandler(
        IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InventoryBalanceDto>> Handle(
        GetInventoryBalancesQuery request,
        CancellationToken cancellationToken)
    {
        var balances =
            await _repository.GetBalancesAsync(
                request.WarehouseId,
                cancellationToken);

        return balances
            .Select(x =>
            {
                var available =
                    x.OnHandQuantity -
                    x.ReservedQuantity;

                return new InventoryBalanceDto
                {
                    WarehouseId =
                        x.WarehouseId,

                    WarehouseCode =
                        x.Warehouse.Code,

                    WarehouseName =
                        x.Warehouse.Name,

                    PPEProductId =
                        x.PPEProductId,

                    Sku =
                        x.PPEProduct.Sku,

                    ProductName =
                        x.PPEProduct.Name,

                    CategoryName =
                        x.PPEProduct.Category.Name,

                    OnHandQuantity =
                        x.OnHandQuantity,

                    ReservedQuantity =
                        x.ReservedQuantity,

                    AvailableQuantity =
                        available,

                    MinimumStock =
                        x.PPEProduct.MinimumStock,

                    IsLowStock =
                        available <=
                        x.PPEProduct.MinimumStock
                };
            })
            .ToArray();
    }
}