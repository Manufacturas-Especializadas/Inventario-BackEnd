using MediatR;

namespace PPEInventory.Application.Features.WarehouseProducts.Commands.Create;

public record CreateWarehouseProductCommand(
    int WarehouseId,
    int PPEProductId)
    : IRequest<WarehouseProductDto>;