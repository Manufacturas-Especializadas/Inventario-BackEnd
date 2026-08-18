using MediatR;

namespace PPEInventory.Application.Features.ProductionLines.Queries.GetAll;

public record GetProductionLinesQuery
    : IRequest<IReadOnlyList<ProductionLineDto>>;