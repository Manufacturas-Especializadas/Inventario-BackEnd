using FluentValidation;

namespace PPEInventory.Application.Features.Users.Commands.SetRoles;

public class SetUserRolesCommandValidator
    : AbstractValidator<SetUserRolesCommand>
{
    public SetUserRolesCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Roles)
            .NotEmpty();

        RuleForEach(x => x.Roles)
            .NotEmpty()
            .MaximumLength(50);
    }
}