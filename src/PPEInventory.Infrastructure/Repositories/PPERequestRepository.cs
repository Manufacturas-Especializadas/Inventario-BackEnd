using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
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
            .Include(x => x.Warehouse)
            .Include(x => x.RequestReason)
            .Include(x => x.Items)
                .ThenInclude(x => x.PPEProduct)
            .FirstOrDefaultAsync(
                x => x.Folio == folio,
                cancellationToken);
    }

    public Task<DateTime?> GetLastDeliveredAtAsync(
        int employeeId,
        int ppeProductId,
        CancellationToken cancellationToken = default)
    {
        return _context.PPERequestItems
            .AsNoTracking()
            .Where(x =>
                x.PPERequest.EmployeeId == employeeId &&
                x.PPEProductId == ppeProductId &&
                x.PPERequest.Status ==
                    PPERequestStatus.Delivered &&
                x.PPERequest.DeliveredAt.HasValue)
            .OrderByDescending(
                x => x.PPERequest.DeliveredAt)
            .Select(
                x => x.PPERequest.DeliveredAt)
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    public Task<PPERequest?> GetByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default)
    {
        return _context.PPERequests
            .Include(x => x.Employee)
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
}