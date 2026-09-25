using FluentValidation;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetEmployeeHistory;

public class GetEmployeePPEHistoryQueryValidator
    : AbstractValidator<GetEmployeePPEHistoryQuery>
{
    public GetEmployeePPEHistoryQueryValidator()
    {
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(
                1,
                PaginationParameters.MaxPageSize);
    }
}