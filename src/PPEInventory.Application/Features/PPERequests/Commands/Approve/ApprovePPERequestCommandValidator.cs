using FluentValidation;

namespace PPEInventory.Application.Features.PPERequests.Commands.Approve;

public class ApprovePPERequestCommandValidator : AbstractValidator<ApprovePPERequestCommand>
{
    public ApprovePPERequestCommandValidator()
    {
        RuleFor(x => x.Folio).NotEmpty().MaximumLength(25);
    }
}
