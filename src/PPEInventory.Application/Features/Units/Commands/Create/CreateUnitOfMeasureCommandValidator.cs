using FluentValidation;

namespace PPEInventory.Application.Features.Units.Commands.Create;

public class CreateUnitOfMeasureCommandValidator
    : AbstractValidator<CreateUnitOfMeasureCommand>
{
    public CreateUnitOfMeasureCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Symbol)
            .MaximumLength(20);
    }
}