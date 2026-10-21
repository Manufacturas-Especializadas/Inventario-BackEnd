using FluentValidation;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPendingApproval;

public class GetPendingApprovalPPERequestsQueryValidator
    : AbstractValidator<GetPendingApprovalPPERequestsQuery>
{
    public GetPendingApprovalPPERequestsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PaginationParameters.MaxPageSize);
    }
}
