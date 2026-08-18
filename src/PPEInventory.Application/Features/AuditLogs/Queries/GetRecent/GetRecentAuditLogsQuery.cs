using MediatR;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.AuditLogs.Queries.GetRecent;

public record GetRecentAuditLogsQuery(
    string? EntityName,
    int? PerformedByUserId,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber = 1,
    int PageSize = PaginationParameters.DefaultPageSize)
    : IRequest<PagedResult<AuditLogDto>>;