using MediatR;

namespace PPEInventory.Application.Features.ProductSizes.Commands.SetStatus;

public record SetProductSizeStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<ProductSizeDto>;