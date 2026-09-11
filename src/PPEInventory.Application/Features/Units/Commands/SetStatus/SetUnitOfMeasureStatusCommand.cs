using MediatR;

namespace PPEInventory.Application.Features.Units.Commands.SetStatus;

public record SetUnitOfMeasureStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<UnitOfMeasureDto>;