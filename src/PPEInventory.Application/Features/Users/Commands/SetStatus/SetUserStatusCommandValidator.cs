using FluentValidation;

namespace PPEInventory.Application.Features.Users.Commands.SetStatus;

public class SetUserStatusCommandValidator
    : AbstractValidator<SetUserStatusCommand>
{
    public SetUserStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}