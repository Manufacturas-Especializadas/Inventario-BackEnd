using MediatR;

namespace PPEInventory.Application.Features.ProductColors.Commands.Update;

public record UpdateProductColorCommand(
    int Id,
    string Name)
    : IRequest<ProductColorDto>;