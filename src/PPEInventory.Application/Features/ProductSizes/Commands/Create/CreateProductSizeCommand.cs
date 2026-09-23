using MediatR;

namespace PPEInventory.Application.Features.ProductSizes.Commands.Create;

public record CreateProductSizeCommand( string Name ): IRequest<ProductSizeDto>;