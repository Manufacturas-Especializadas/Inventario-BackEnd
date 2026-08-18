using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class AuditLogRepository
    : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(
            auditLog,
            cancellationToken);
    }

    public async Task<PagedResult<AuditLog>> GetPagedAsync(
        string? entityName,
        int? performedByUserId,
        DateTime? dateFrom,
        DateTime? dateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.AuditLogs
                .AsNoTracking()
                .Include(x => x.PerformedByUser)
                    .ThenInclude(x => x.Employee)
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(
                x => x.EntityName == entityName);
        }

        if (performedByUserId.HasValue)
        {
            query = query.Where(
                x =>
                    x.PerformedByUserId ==
                    performedByUserId.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(
                x =>
                    x.CreatedAt >=
                    dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(
                x =>
                    x.CreatedAt <=
                    dateTo.Value);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(
                    x => x.CreatedAt)
                .ThenByDescending(
                    x => x.Id)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(pageSize)
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<AuditLog>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}