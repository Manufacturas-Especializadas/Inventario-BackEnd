using MediatR;

namespace PPEInventory.Application.Features.PPEProducts.Commands.SetStatus;

public record SetPPEProductStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<PPEProductDto>;