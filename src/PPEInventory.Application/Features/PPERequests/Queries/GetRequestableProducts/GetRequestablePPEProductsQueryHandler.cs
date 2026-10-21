using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetRequestableProducts;

public class GetRequestablePPEProductsQueryHandler
    : IRequestHandler<GetRequestablePPEProductsQuery, IReadOnlyList<RequestablePPEProductDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWarehouseProductRepository _warehouseProductRepository;

    public GetRequestablePPEProductsQueryHandler(
        IWarehouseRepository warehouseRepository,
        IWarehouseProductRepository warehouseProductRepository)
    {
        _warehouseRepository = warehouseRepository;
        _warehouseProductRepository = warehouseProductRepository;
    }

    public async Task<IReadOnlyList<RequestablePPEProductDto>> Handle(
        GetRequestablePPEProductsQuery request,
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(
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

        return await _warehouseProductRepository.GetRequestableProductsAsync(
            warehouse.Id,
            cancellationToken);
    }
}
