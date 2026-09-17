using MediatR;

namespace PPEInventory.Application.Features.WarehouseProducts.Commands.BulkAssign;

public record BulkAssignWarehouseProductsCommand(
    IReadOnlyCollection<int> WarehouseIds,
    IReadOnlyCollection<int> PPEProductIds)
    : IRequest<BulkAssignWarehouseProductsResultDto>;