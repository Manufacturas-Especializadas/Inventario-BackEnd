using MediatR;

namespace PPEInventory.Application.Features.Units.Commands.Update;

public record UpdateUnitOfMeasureCommand(
    int Id,
    string Name,
    string? Symbol)
    : IRequest<UnitOfMeasureDto>;