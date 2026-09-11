using FluentValidation;

namespace PPEInventory.Application.Features.PPECategories.Commands.SetStatus;

public class SetPPECategoryStatusCommandValidator
    : AbstractValidator<SetPPECategoryStatusCommand>
{
    public SetPPECategoryStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}