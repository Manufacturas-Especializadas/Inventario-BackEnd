using MediatR;

namespace PPEInventory.Application.Features.WarehouseProducts.Commands.ChangeStatus;

public record ChangeWarehouseProductStatusCommand(
    int WarehouseId,
    int PPEProductId,
    bool IsActive)
    : IRequest<WarehouseProductDto>;