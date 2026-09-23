using FluentValidation;

namespace PPEInventory.Application.Features.ProductSizes.Commands.Create;

public class CreateProductSizeCommandValidator
    : AbstractValidator<CreateProductSizeCommand>
{
    public CreateProductSizeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}