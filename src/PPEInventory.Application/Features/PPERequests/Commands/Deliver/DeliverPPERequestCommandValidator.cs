using FluentValidation;

namespace PPEInventory.Application.Features.PPERequests.Commands.Deliver;

public class DeliverPPERequestCommandValidator
    : AbstractValidator<DeliverPPERequestCommand>
{
    public DeliverPPERequestCommandValidator()
    {
        RuleFor(x => x.Folio)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(x => x.EmployeeNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}