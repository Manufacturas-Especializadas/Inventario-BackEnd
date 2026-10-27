using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Features.PPERequests;
using PPEInventory.Application.Features.PPERequests.Queries.GetDeliveryHistory;
using PPEInventory.Application.Features.PPERequests.Queries.GetPending;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Constants;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class PPERequestRepository
    : IPPERequestRepository
{
    private readonly ApplicationDbContext _context;

    public PPERequestRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> CountByStatusAsync(
        PPERequestStatus status,
        CancellationToken cancellationToken = default)
    {
        return _context.PPERequests.CountAsync(
            x => x.Status == status,
            cancellationToken);
    }

    public async Task AddAsync(
        PPERequest request,
        CancellationToken cancellationToken = default)
    {
        await _context.PPERequests.AddAsync(
            request,
            cancellationToken);
    }

    public Task<PPERequest?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default)
    {
        return _context.PPERequests
            .AsNoTracking()
            .Include(x => x.Employee)
                .ThenInclude(x => x.Department)
            .Include(x => x.Employee)
                .ThenInclude(x => x.Line)
            .Include(x => x.RequestedForOrganizationalUnit)
            .Include(x => x.Warehouse)
            .Include(x => x.RequestReason)
            .Include(x => x.ApprovedByUser)
                .ThenInclude(x => x!.Employee)
            .Include(x => x.DeliveredByUser)
                .ThenInclude(x => x!.Employee)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
                    .ThenInclude(x => x.StockUnitOfMeasure)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }

    public Task<PPERequest?> GetByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default)
    {
        return _context.PPERequests
            .FromSqlInterpolated($"""
            SELECT *
            FROM PPERequests WITH (UPDLOCK, HOLDLOCK)
            WHERE Folio = {folio}
            """)
            .Include(x => x.Employee)
            .Include(x => x.RequestedForOrganizationalUnit)
            .Include(x => x.Warehouse)
            .Include(x => x.RequestReason)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PPERequest>>
    GetPendingByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.PPERequests
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.RequestedForOrganizationalUnit)
            .Include(x => x.Warehouse)
            .Include(x => x.RequestReason)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .Where(x =>
                x.Employee.EmployeeNumber == employeeNumber &&
                x.Status == PPERequestStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<PendingPPERequestDto>> GetPendingAsync(
        GetPendingPPERequestsQuery filters,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.PPERequests
                .AsNoTracking()
                // Same workflow state accepted by DeliverPPERequestCommandHandler.
                // PendingApproval, Rejected, Cancelled and Delivered are excluded.
                .Where(x =>
                    x.Status == PPERequestStatus.Pending)
                .AsQueryable();

        if (filters.WarehouseId.HasValue)
        {
            query = query.Where(
                x => x.WarehouseId == filters.WarehouseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim();
            query = query.Where(x =>
                x.Folio.Contains(search) ||
                x.Employee.EmployeeNumber.Contains(search) ||
                x.Employee.Name.Contains(search));
        }

        if (filters.RequestedForOrganizationalUnitId.HasValue)
        {
            query = query.Where(x => x.RequestedForOrganizationalUnitId ==
                filters.RequestedForOrganizationalUnitId.Value);
        }

        if (filters.DateFrom.HasValue)
        {
            var from = DateTime.SpecifyKind(
                filters.DateFrom.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt >= from);
        }

        // An exclusive upper bound includes the entire final day. MaxValue has
        // no following day, and already includes every representable timestamp.
        if (filters.DateTo.HasValue && filters.DateTo.Value < DateOnly.MaxValue)
        {
            var toExclusive = DateTime.SpecifyKind(
                filters.DateTo.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt < toExclusive);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(x => new PendingPPERequestDto
            {
                Id = x.Id,
                Folio = x.Folio,
                CreatedAt = x.CreatedAt,
                EmployeeId = x.EmployeeId,
                EmployeeNumber = x.Employee.EmployeeNumber,
                EmployeeName = x.Employee.Name,
                RequestedForOrganizationalUnitId = x.RequestedForOrganizationalUnitId,
                RequestedForOrganizationalUnitName = x.RequestedForOrganizationalUnit == null
                    ? null : x.RequestedForOrganizationalUnit.Name,
                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,
                RequestReasonId = x.RequestReasonId,
                RequestReason = x.RequestReason.Name,
                RequestReasonCode = x.RequestReason.Code,
                ItemCount = x.Items.Count,
                Status = x.Status,
                ApprovedAt = x.ApprovedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PendingPPERequestDto>
        {
            Items = items,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<PendingApprovalPPERequestDto>> GetPendingApprovalPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.PPERequests
            .AsNoTracking()
            .Where(x => x.Status == PPERequestStatus.PendingApproval);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .AsSingleQuery()
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PendingApprovalPPERequestDto
            {
                Id = x.Id,
                Folio = x.Folio,
                Status = x.Status,
                EmployeeId = x.EmployeeId,
                EmployeeNumber = x.Employee.EmployeeNumber,
                EmployeeName = x.Employee.Name,
                RequestedForOrganizationalUnitId = x.RequestedForOrganizationalUnitId,
                RequestedForOrganizationalUnitName = x.RequestedForOrganizationalUnit == null
                    ? null
                    : x.RequestedForOrganizationalUnit.Name,
                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,
                RequestReasonId = x.RequestReasonId,
                RequestReason = x.RequestReason.Name,
                RequestReasonCode = x.RequestReason.Code,
                Notes = x.Notes,
                CreatedByUserId = x.CreatedByUserId,
                CreatedAt = x.CreatedAt,
                Items = x.Items.OrderBy(item => item.Id)
                    .Select(item => new PendingApprovalPPERequestItemDto
                    {
                        PPEProductId = item.PPEProductId,
                        Sku = item.PPEProduct.Sku,
                        ProductName = item.PPEProduct.Name,
                        Quantity = item.Quantity
                    }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PendingApprovalPPERequestDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyDictionary<int, int>>
    GetCommittedNormalQuantitiesInCycleAsync(
        int requestedForOrganizationalUnitId,
        IReadOnlyCollection<int> ppeProductIds,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var productIds =
            ppeProductIds
                .Distinct()
                .ToArray();

        if (productIds.Length == 0)
        {
            return new Dictionary<int, int>();
        }

        return await _context.PPERequestItems
            .AsNoTracking()
            .Where(x =>
                x.PPERequest.RequestedForOrganizationalUnitId ==
                    requestedForOrganizationalUnitId &&

                productIds.Contains(
                    x.PPEProductId) &&

                x.PPERequest.RequestReason.Code !=
                    RequestReasonCodes.Damage &&

                x.PPERequest.RequestReason.Code !=
                    RequestReasonCodes.Lost &&

                x.PPERequest.RequestReason.Code !=
                    RequestReasonCodes.Other &&

                (
                    x.PPERequest.Status ==
                        PPERequestStatus.Pending
                    ||
                    (
                        x.PPERequest.Status ==
                            PPERequestStatus.Delivered &&

                        x.PPERequest.DeliveredAt.HasValue &&

                        x.PPEProduct
                            .ReplacementIntervalDays
                            .HasValue &&

                        x.PPEProduct
                            .ReplacementIntervalDays
                            .Value > 0 &&

                        x.PPERequest.DeliveredAt.Value >=
                            now.AddDays(
                                -x.PPEProduct
                                    .ReplacementIntervalDays
                                    .Value)
                    )
                ))
            .GroupBy(x =>
                x.PPEProductId)
            .Select(group =>
                new
                {
                    ProductId =
                        group.Key,

                    Quantity =
                        group.Sum(x =>
                            x.Quantity)
                })
            .ToDictionaryAsync(
                x => x.ProductId,
                x => x.Quantity,
                cancellationToken);
    }

    public async Task<PagedResult<PPERequest>>
    GetHistoryPageByEmployeeNumberAsync(
        string employeeNumber,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.PPERequests
                .AsNoTracking()
                .Where(x =>
                    x.Employee.EmployeeNumber ==
                        employeeNumber);

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .Include(x => x.Employee)
                .Include(x =>
                    x.RequestedForOrganizationalUnit)
                .Include(x => x.Warehouse)
                .Include(x => x.RequestReason)
                .Include(x => x.Items)
                .ThenInclude(x =>
                    x.PPEProduct)
                    .ThenInclude(x =>
                        x.StockUnitOfMeasure)
                .OrderByDescending(x =>
                    x.CreatedAt)
                .ThenByDescending(x =>
                    x.Id)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(pageSize)
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<PPERequest>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<DeliveryHistoryPPERequestDto>>
    GetDeliveryHistoryAsync(
        GetDeliveryHistoryQuery filters,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.PPERequests
                .AsNoTracking()
                .Where(x =>
                    x.Status ==
                        PPERequestStatus.Delivered &&
                    x.DeliveredAt.HasValue)
                .AsQueryable();

        // Almacén
        if (filters.WarehouseId.HasValue)
        {
            query = query.Where(
                x =>
                    x.WarehouseId ==
                    filters.WarehouseId.Value);
        }

        // Nómina exacta
        if (!string.IsNullOrWhiteSpace(
            filters.EmployeeNumber))
        {
            var employeeNumber =
                filters.EmployeeNumber.Trim();

            query = query.Where(
                x =>
                    x.Employee.EmployeeNumber ==
                    employeeNumber);
        }

        // Folio o nombre del empleado
        if (!string.IsNullOrWhiteSpace(
            filters.Search))
        {
            var search =
                filters.Search.Trim();

            query = query.Where(
                x =>
                    x.Folio.Contains(search) ||
                    x.Employee.Name.Contains(
                        search));
        }

        // Unidad organizacional / línea destino
        if (filters
            .RequestedForOrganizationalUnitId
            .HasValue)
        {
            query = query.Where(
                x =>
                    x.RequestedForOrganizationalUnitId ==
                    filters
                        .RequestedForOrganizationalUnitId
                        .Value);
        }

        // Desde
        var from =
            DateTime.SpecifyKind(
                filters.DeliveredFrom
                    .ToDateTime(
                        TimeOnly.MinValue),
                DateTimeKind.Utc);

        query = query.Where(
            x =>
                x.DeliveredAt >= from);

        // Hasta
        if (filters.DeliveredTo <
            DateOnly.MaxValue)
        {
            var toExclusive =
                DateTime.SpecifyKind(
                    filters.DeliveredTo
                        .AddDays(1)
                        .ToDateTime(
                            TimeOnly.MinValue),
                    DateTimeKind.Utc);

            query = query.Where(
                x =>
                    x.DeliveredAt <
                    toExclusive);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(
                    x => x.DeliveredAt)
                .ThenByDescending(
                    x => x.Id)
                .Skip(
                    (filters.PageNumber - 1) *
                    filters.PageSize)
                .Take(
                    filters.PageSize)
                .Select(
                    x =>
                        new DeliveryHistoryPPERequestDto
                        {
                            Id =
                                x.Id,

                            Folio =
                                x.Folio,

                            CreatedAt =
                                x.CreatedAt,

                            DeliveredAt =
                                x.DeliveredAt!.Value,

                            EmployeeId =
                                x.EmployeeId,

                            EmployeeNumber =
                                x.Employee
                                    .EmployeeNumber,

                            EmployeeName =
                                x.Employee.Name,

                            RequestedForOrganizationalUnitId =
                                x.RequestedForOrganizationalUnitId,

                            RequestedForOrganizationalUnitName =
                                x.RequestedForOrganizationalUnit ==
                                null
                                    ? null
                                    : x.RequestedForOrganizationalUnit
                                        .Name,

                            WarehouseId =
                                x.WarehouseId,

                            WarehouseName =
                                x.Warehouse.Name,

                            RequestReasonId =
                                x.RequestReasonId,

                            RequestReason =
                                x.RequestReason.Name,

                            RequestReasonCode =
                                x.RequestReason.Code,

                            ItemCount =
                                x.Items.Count,

                            Status =
                                x.Status,

                            DeliveredByUserId =
                                x.DeliveredByUserId,

                            DeliveredByUserName =
                                x.DeliveredByUser ==
                                null
                                    ? null
                                    : x.DeliveredByUser
                                        .Employee.Name
                        })
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<
            DeliveryHistoryPPERequestDto>
        {
            Items =
                items,

            PageNumber =
                filters.PageNumber,

            PageSize =
                filters.PageSize,

            TotalCount =
                totalCount
        };
    }



}
