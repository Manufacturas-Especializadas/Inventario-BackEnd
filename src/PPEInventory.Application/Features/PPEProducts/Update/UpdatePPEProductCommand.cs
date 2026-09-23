using MediatR;

namespace PPEInventory.Application.Features.PPEProducts.Commands.Update;

public record UpdatePPEProductCommand(
    int Id,
    int CategoryId,
    string Name,
    string? Description,
    int? SizeId,
    int? ColorId,
    string? Model,
    string? Specification,
    int StockUnitId,
    int MinimumStock,
    int? DefaultMaxQuantityPerCycle,
    int? ReplacementIntervalDays)
    : IRequest<PPEProductDto>;