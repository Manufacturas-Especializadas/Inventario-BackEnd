using MediatR;

namespace PPEInventory.Application.Features.PPECategories.Commands.Update;

public record UpdatePPECategoryCommand(
    int Id,
    string Name,
    string? Description)
    : IRequest<PPECategoryDto>;