using MediatR;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.AuditLogs.Queries.GetRecent;

public class GetRecentAuditLogsQueryHandler
    : IRequestHandler<
        GetRecentAuditLogsQuery,
        PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _repository;

    public GetRecentAuditLogsQueryHandler(
        IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(
        GetRecentAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var entityName =
            string.IsNullOrWhiteSpace(
                request.EntityName)
                ? null
                : request.EntityName.Trim();

        var result =
            await _repository.GetPagedAsync(
                entityName,
                request.PerformedByUserId,
                request.DateFrom,
                request.DateTo,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            PageNumber =
                result.PageNumber,

            PageSize =
                result.PageSize,

            TotalCount =
                result.TotalCount,

            Items =
                result.Items
                    .Select(x =>
                        new AuditLogDto
                        {
                            Id =
                                x.Id,

                            EntityName =
                                x.EntityName,

                            EntityId =
                                x.EntityId,

                            Action =
                                x.Action,

                            Description =
                                x.Description,

                            OldValuesJson =
                                x.OldValuesJson,

                            NewValuesJson =
                                x.NewValuesJson,

                            PerformedByUserId =
                                x.PerformedByUserId,

                            PerformedByUsername =
                                x.PerformedByUser.Username,

                            PerformedByEmployeeName =
                                x.PerformedByUser.Employee.Name,

                            CreatedAt =
                                x.CreatedAt
                        })
                    .ToArray()
        };
    }
}