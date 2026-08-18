using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Inventory.Queries.GetLowStock;

public class GetLowStockQueryHandler
    : IRequestHandler<
        GetLowStockQuery,
        IReadOnlyList<LowStockDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IPPEProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public GetLowStockQueryHandler(
        IWarehouseRepository warehouseRepository,
        IPPEProductRepository productRepository,
        IInventoryRepository inventoryRepository)
    {
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IReadOnlyList<LowStockDto>> Handle(
        GetLowStockQuery request,
        CancellationToken cancellationToken)
    {
        var warehouse =
            await _warehouseRepository.GetByIdAsync(
                request.WarehouseId,
                cancellationToken);

        if (warehouse is null)
        {
            throw new NotFoundException(
                $"Warehouse with id '{request.WarehouseId}' was not found.");
        }

        var products =
            await _productRepository.GetAllAsync(
                cancellationToken);

        var balances =
            await _inventoryRepository.GetBalancesAsync(
                warehouse.Id,
                cancellationToken);

        var balanceByProductId =
            balances.ToDictionary(
                x => x.PPEProductId);

        var result =
            new List<LowStockDto>();

        foreach (var product in
            products.Where(x => x.IsActive))
        {
            balanceByProductId.TryGetValue(
                product.Id,
                out var balance);

            var onHand =
                balance?.OnHandQuantity ?? 0;

            var reserved =
                balance?.ReservedQuantity ?? 0;

            var available =
                onHand - reserved;

            if (available > product.MinimumStock)
            {
                continue;
            }

            result.Add(
                new LowStockDto
                {
                    WarehouseId =
                        warehouse.Id,

                    WarehouseCode =
                        warehouse.Code,

                    WarehouseName =
                        warehouse.Name,

                    PPEProductId =
                        product.Id,

                    Sku =
                        product.Sku,

                    ProductName =
                        product.Name,

                    CategoryName =
                        product.Category.Name,

                    OnHandQuantity =
                        onHand,

                    ReservedQuantity =
                        reserved,

                    AvailableQuantity =
                        available,

                    MinimumStock =
                        product.MinimumStock,

                    ShortageQuantity =
                        Math.Max(
                            product.MinimumStock -
                            available,
                            0)
                });
        }

        return result
            .OrderByDescending(
                x => x.ShortageQuantity)
            .ThenBy(x => x.ProductName)
            .ToArray();
    }
}