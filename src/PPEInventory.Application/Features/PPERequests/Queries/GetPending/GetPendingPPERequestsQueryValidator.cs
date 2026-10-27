using FluentValidation;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPending;

public class GetPendingPPERequestsQueryValidator
    : AbstractValidator<GetPendingPPERequestsQuery>
{
    public GetPendingPPERequestsQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .When(x => x.WarehouseId.HasValue);

        RuleFor(x => x.RequestedForOrganizationalUnitId)
            .GreaterThan(0)
            .When(x => x.RequestedForOrganizationalUnitId.HasValue);

        RuleFor(x => x.Search).MaximumLength(150);

        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PaginationParameters.MaxPageSize);

        RuleFor(x => x)
            .Must(x => ((long)x.PageNumber - 1) * x.PageSize <= int.MaxValue)
            .When(x => x.PageNumber > 0 && x.PageSize > 0)
            .WithMessage("The requested pagination offset is too large.");

        RuleFor(x => x)
            .Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue ||
                x.DateFrom.Value <= x.DateTo.Value)
            .WithMessage("DateFrom cannot be greater than DateTo.");
    }
}
