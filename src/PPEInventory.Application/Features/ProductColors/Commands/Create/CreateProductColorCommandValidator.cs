using FluentValidation;

namespace PPEInventory.Application.Features.ProductColors.Commands.Create;

public class CreateProductColorCommandValidator
    : AbstractValidator<CreateProductColorCommand>
{
    public CreateProductColorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}