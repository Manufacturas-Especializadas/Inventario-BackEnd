using FluentValidation;

namespace PPEInventory.Application.Features.PPECategories.Commands.Create;

public class CreatePPECategoryCommandValidator
    : AbstractValidator<CreatePPECategoryCommand>
{
    public CreatePPECategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}