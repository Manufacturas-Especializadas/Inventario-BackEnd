using MediatR;

namespace PPEInventory.Application.Features.ProductColors.Commands.Create;

public record CreateProductColorCommand(
    string Name
) : IRequest<ProductColorDto>;