using MediatR;

namespace PPEInventory.Application.Features.PPECategories.Commands.Create;

public record CreatePPECategoryCommand(
    string Name,
    string? Description)
    : IRequest<PPECategoryDto>;