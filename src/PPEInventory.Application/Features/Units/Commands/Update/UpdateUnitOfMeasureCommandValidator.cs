using FluentValidation;

namespace PPEInventory.Application.Features.Units.Commands.Update;

public class UpdateUnitOfMeasureCommandValidator
    : AbstractValidator<UpdateUnitOfMeasureCommand>
{
    public UpdateUnitOfMeasureCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Symbol)
            .MaximumLength(20);
    }
}
