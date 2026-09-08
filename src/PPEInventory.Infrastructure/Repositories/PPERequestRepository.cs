using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;
using PPEInventory.Infrastructure.Persistence;
using PPEInventory.Domain.Constants;

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
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }

    public Task<PPERequest?> GetByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default)
    {
        return _context.PPERequests
            .Include(x => x.Employee)
            .Include(x => x.RequestedForOrganizationalUnit)
            .Include(x => x.Warehouse)
            .Include(x => x.RequestReason)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
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

    public async Task<IReadOnlyList<PPERequest>> GetPendingAsync(
    int? warehouseId,
    CancellationToken cancellationToken = default)
    {
        var query =
            _context.PPERequests
                .AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.RequestedForOrganizationalUnit)
                .Include(x => x.Warehouse)
                .Include(x => x.RequestReason)
                .Include(x => x.Items)
                    .ThenInclude(x => x.PPEProduct)
                .Where(x =>
                    x.Status == PPERequestStatus.Pending)
                .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(
                x => x.WarehouseId == warehouseId.Value);
        }

        return await query
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PPERequest>>
    GetHistoryByEmployeeNumberAsync(
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
                x.Employee.EmployeeNumber == employeeNumber)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetCommittedNormalQuantityInCycleAsync(
    int requestedForOrganizationalUnitId,
    int ppeProductId,
    DateTime cycleStart,
    CancellationToken cancellationToken = default)
    {
        return _context.PPERequestItems
            .AsNoTracking()
            .Where(x =>
                x.PPERequest.RequestedForOrganizationalUnitId ==
    requestedForOrganizationalUnitId &&

                x.PPEProductId ==
                    ppeProductId &&

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

                        x.PPERequest.DeliveredAt.Value >=
                            cycleStart
                    )
                ))
            .SumAsync(
                x => x.Quantity,
                cancellationToken);
    }
}