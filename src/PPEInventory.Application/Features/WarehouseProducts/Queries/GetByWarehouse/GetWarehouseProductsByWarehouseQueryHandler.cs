using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.WarehouseProducts.Queries.GetByWarehouse;

public class GetWarehouseProductsByWarehouseQueryHandler
    : IRequestHandler<
        GetWarehouseProductsByWarehouseQuery,
        IReadOnlyList<WarehouseProductDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWarehouseProductRepository _repository;

    public GetWarehouseProductsByWarehouseQueryHandler(
        IWarehouseRepository warehouseRepository,
        IWarehouseProductRepository repository)
    {
        _warehouseRepository = warehouseRepository;
        _repository = repository;
    }

    public async Task<IReadOnlyList<WarehouseProductDto>> Handle(
        GetWarehouseProductsByWarehouseQuery request,
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

        var relations =
            await _repository.GetByWarehouseIdAsync(
                warehouse.Id,
                cancellationToken);

        return relations
            .Select(x => x.ToDto())
            .ToArray();
    }
}