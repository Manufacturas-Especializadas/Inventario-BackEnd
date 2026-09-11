using MediatR;

namespace PPEInventory.Application.Features.Units.Queries.GetAll;

public record GetUnitsQuery: IRequest<IReadOnlyList<UnitOfMeasureDto>>;