using MediatR;

namespace PPEInventory.Application.Features.PPECategories.Commands.SetStatus;

public record SetPPECategoryStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<PPECategoryDto>;