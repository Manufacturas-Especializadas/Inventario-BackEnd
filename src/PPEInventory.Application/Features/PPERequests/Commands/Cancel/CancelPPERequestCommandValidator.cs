using FluentValidation;

namespace PPEInventory.Application.Features.PPERequests.Commands.Cancel;

public class CancelPPERequestCommandValidator
    : AbstractValidator<CancelPPERequestCommand>
{
    public CancelPPERequestCommandValidator()
    {
        RuleFor(x => x.Folio)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required.")
            .MaximumLength(500);
    }
}