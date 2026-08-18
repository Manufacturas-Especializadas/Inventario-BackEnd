using MediatR;

namespace PPEInventory.Application.Features.Warehouses.Commands.Create;

public record CreateWarehouseCommand(
    string Code,
    string Name,
    string? Description)
    : IRequest<WarehouseDto>;