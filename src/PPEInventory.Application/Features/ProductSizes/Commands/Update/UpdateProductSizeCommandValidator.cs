using FluentValidation;

namespace PPEInventory.Application.Features.ProductSizes.Commands.Update;

public class UpdateProductSizeCommandValidator
    : AbstractValidator<UpdateProductSizeCommand>
{
    public UpdateProductSizeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}