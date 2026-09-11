using FluentValidation;

namespace PPEInventory.Application.Features.PPECategories.Commands.Update;

public class UpdatePPECategoryCommandValidator
    : AbstractValidator<UpdatePPECategoryCommand>
{
    public UpdatePPECategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .When(
                x =>
                    !string.IsNullOrWhiteSpace(
                        x.Description));
    }
}