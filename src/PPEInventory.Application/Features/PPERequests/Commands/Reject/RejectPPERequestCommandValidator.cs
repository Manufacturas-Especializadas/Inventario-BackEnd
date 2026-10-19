using FluentValidation;

namespace PPEInventory.Application.Features.PPERequests.Commands.Reject;

public class RejectPPERequestCommandValidator : AbstractValidator<RejectPPERequestCommand>
{
    public RejectPPERequestCommandValidator()
    {
        RuleFor(x => x.Folio).NotEmpty().MaximumLength(25);
        RuleFor(x => x.RejectionReason).NotEmpty().MaximumLength(500);
    }
}
