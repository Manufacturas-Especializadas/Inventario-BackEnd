using MediatR;

namespace PPEInventory.Application.Features.PPEProducts.Commands.Create;

public record CreatePPEProductCommand(
    int CategoryId,
    string Name,
    string? Description,
    int? SizeId,
    string? Color,
    string? Model,
    string? Specification,
    int StockUnitId,
    int MinimumStock,
    int? DefaultMaxQuantityPerCycle,
    int? ReplacementIntervalDays)
    : IRequest<PPEProductDto>;