using MediatR;

namespace PPEInventory.Application.Features.ProductionLines.Commands.Create;

public record CreateProductionLineCommand(
    int DepartmentId,
    string Name,
    string? Description)
    : IRequest<ProductionLineDto>;