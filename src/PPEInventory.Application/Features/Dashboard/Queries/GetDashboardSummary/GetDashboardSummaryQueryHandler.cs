using MediatR;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IPPERequestRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardSummaryQueryHandler(
        IPPERequestRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var isAdministrator = _currentUser.IsInRole(AppRoles.Administrator);
        int? pendingDeliveries = null;
        int? pendingApprovals = null;

        if (isAdministrator || _currentUser.IsInRole(AppRoles.Warehouse))
        {
            pendingDeliveries = await _repository.CountByStatusAsync(
                PPERequestStatus.Pending, cancellationToken);
        }

        // Sequential queries share the scoped EF Core DbContext.
        if (isAdministrator)
        {
            pendingApprovals = await _repository.CountByStatusAsync(
                PPERequestStatus.PendingApproval, cancellationToken);
        }

        return new DashboardSummaryDto
        {
            PendingDeliveries = pendingDeliveries,
            PendingApprovals = pendingApprovals
        };
    }
}
