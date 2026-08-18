using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.ProductionLines.Queries.GetAll;

public class GetProductionLinesQueryHandler
    : IRequestHandler<
        GetProductionLinesQuery,
        IReadOnlyList<ProductionLineDto>>
{
    private readonly IProductionLineRepository _repository;

    public GetProductionLinesQueryHandler(
        IProductionLineRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductionLineDto>> Handle(
        GetProductionLinesQuery request,
        CancellationToken cancellationToken)
    {
        var productionLines =
            await _repository.GetAllAsync(
                cancellationToken);

        return productionLines
            .Select(x => new ProductionLineDto
            {
                Id = x.Id,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department.Name,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}