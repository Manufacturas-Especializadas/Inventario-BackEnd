using FluentValidation;

namespace PPEInventory.Application.Features.ProductColors.Commands.Update;

public class UpdateProductColorCommandValidator
    : AbstractValidator<UpdateProductColorCommand>
{
    public UpdateProductColorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}