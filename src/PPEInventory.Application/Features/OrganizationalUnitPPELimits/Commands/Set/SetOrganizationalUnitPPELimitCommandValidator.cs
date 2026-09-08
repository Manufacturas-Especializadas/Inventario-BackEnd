using FluentValidation;

namespace PPEInventory.Application.Features.OrganizationalUnitPPELimits.Commands.Set;

public class SetOrganizationalUnitPPELimitCommandValidator
    : AbstractValidator<
        SetOrganizationalUnitPPELimitCommand>
{
    public SetOrganizationalUnitPPELimitCommandValidator()
    {
        RuleFor(
                x =>
                    x.OrganizationalUnitId)
            .GreaterThan(0);

        RuleFor(x => x.PPEProductId)
            .GreaterThan(0);

        RuleFor(
                x =>
                    x.MaxQuantityPerCycle)
            .GreaterThan(0);
    }
}