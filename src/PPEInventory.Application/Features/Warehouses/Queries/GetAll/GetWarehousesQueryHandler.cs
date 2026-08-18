using MediatR;
using PPEInventory.Application.Features.Suppliers;
using PPEInventory.Application.Features.Suppliers.Queries.GetAll;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Warehouses.Queries.GetAll;

public class GetWarehousesQueryHandler
    : IRequestHandler<
        GetWarehousesQuery,
        IReadOnlyList<WarehouseDto>>
{
    private readonly IWarehouseRepository _repository;

    public GetWarehousesQueryHandler(
        IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<WarehouseDto>> Handle(
        GetWarehousesQuery request,
        CancellationToken cancellationToken)
    {
        var warehouses =
            await _repository.GetAllAsync(
                cancellationToken);

        return warehouses
            .Select(x => new WarehouseDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}