using FluentValidation;

namespace PPEInventory.Application.Features.PPEProducts.Commands.Create;

public class CreatePPEProductCommandValidator
    : AbstractValidator<CreatePPEProductCommand>
{
    public CreatePPEProductCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.SizeId)
            .GreaterThan(0)
            .When(x => x.SizeId.HasValue);

        RuleFor(x => x.Color)
            .MaximumLength(50);

        RuleFor(x => x.Model)
            .MaximumLength(100);

        RuleFor(x => x.Specification)
            .MaximumLength(250);

        RuleFor(x => x.StockUnitId)
            .GreaterThan(0);

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DefaultMaxQuantityPerCycle)
            .GreaterThan(0)
            .When(x => x.DefaultMaxQuantityPerCycle.HasValue);

        RuleFor(x => x.ReplacementIntervalDays)
            .GreaterThan(0)
            .When(x => x.ReplacementIntervalDays.HasValue);
    }
}