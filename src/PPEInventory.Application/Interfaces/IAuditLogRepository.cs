using PPEInventory.Application.Common.Models;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLog>> GetPagedAsync(
        string? entityName,
        int? performedByUserId,
        DateTime? dateFrom,
        DateTime? dateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}