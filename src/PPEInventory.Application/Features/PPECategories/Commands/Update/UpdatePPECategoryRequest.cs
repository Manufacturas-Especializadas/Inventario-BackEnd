namespace PPEInventory.Application.Features.PPECategories.Commands.Update;

public record UpdatePPECategoryRequest(
    string Name,
    string? Description);