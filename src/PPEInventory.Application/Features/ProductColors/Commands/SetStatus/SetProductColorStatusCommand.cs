using MediatR;

namespace PPEInventory.Application.Features.ProductColors.Commands.SetStatus;

public record SetProductColorStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<ProductColorDto>;