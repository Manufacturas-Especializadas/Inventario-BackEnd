using MediatR;

namespace PPEInventory.Application.Features.PPEProducts.Commands.Create;

public record CreatePPEProductCommand(
    int CategoryId,
    string Name,
    string? Description,
    string? Size,
    string? Color,
    string? Model,
    string? Specification,
    string StockUnit,
    int MinimumStock,
    int? MaxQuantityPerRequest,
    int? ReplacementIntervalDays)
    : IRequest<PPEProductDto>;