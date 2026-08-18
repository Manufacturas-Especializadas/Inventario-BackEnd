using FluentValidation;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.AuditLogs.Queries.GetRecent;

public class GetRecentAuditLogsQueryValidator
    : AbstractValidator<GetRecentAuditLogsQuery>
{
    public GetRecentAuditLogsQueryValidator()
    {
        RuleFor(x => x.EntityName)
            .MaximumLength(100);

        RuleFor(x => x.PerformedByUserId)
            .GreaterThan(0)
            .When(x =>
                x.PerformedByUserId.HasValue);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(
                1,
                PaginationParameters.MaxPageSize);

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(
                x => x.DateFrom)
            .When(x =>
                x.DateFrom.HasValue &&
                x.DateTo.HasValue);
    }
}