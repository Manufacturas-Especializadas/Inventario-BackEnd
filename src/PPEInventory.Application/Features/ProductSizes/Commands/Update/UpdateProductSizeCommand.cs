using MediatR;

namespace PPEInventory.Application.Features.ProductSizes.Commands.Update;

public record UpdateProductSizeCommand(
    int Id,
    string Name)
    : IRequest<ProductSizeDto>;