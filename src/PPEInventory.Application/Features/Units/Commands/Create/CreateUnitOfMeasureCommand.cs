using MediatR;

namespace PPEInventory.Application.Features.Units.Commands.Create;

public record CreateUnitOfMeasureCommand(
    string Name,
    string? Symbol)
    : IRequest<UnitOfMeasureDto>;