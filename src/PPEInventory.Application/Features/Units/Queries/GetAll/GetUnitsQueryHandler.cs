using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Units.Queries.GetAll;

public class GetUnitsQueryHandler
    : IRequestHandler<
        GetUnitsQuery,
        IReadOnlyList<UnitOfMeasureDto>>
{
    private readonly IUnitOfMeasureRepository _repository;

    public GetUnitsQueryHandler(
        IUnitOfMeasureRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<UnitOfMeasureDto>> Handle(
        GetUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var units =
            await _repository.GetAllAsync(
                cancellationToken);

        return units
            .Select(x => new UnitOfMeasureDto
            {
                Id = x.Id,
                Name = x.Name,
                Symbol = x.Symbol,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToList();
    }
}