using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class InventoryRepository
    : IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<InventoryBalance?> GetBalanceAsync(
        int warehouseId,
        int ppeProductId,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryBalances
            .FirstOrDefaultAsync(
                x =>
                    x.WarehouseId == warehouseId &&
                    x.PPEProductId == ppeProductId,
                cancellationToken);
    }

    public async Task AddBalanceAsync(
        InventoryBalance balance,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryBalances.AddAsync(
            balance,
            cancellationToken);
    }

    public Task AddMovementsAsync(
        IEnumerable<InventoryMovement> movements,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryMovements.AddRangeAsync(
            movements,
            cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryBalance>>
        GetBalancesAsync(
            int? warehouseId,
            CancellationToken cancellationToken = default)
    {
        var query =
            _context.InventoryBalances
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.PPEProduct)
                    .ThenInclude(x => x.Category)
                .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(
                x => x.WarehouseId == warehouseId);
        }

        return await query
            .OrderBy(x => x.PPEProduct.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<InventoryMovement>>
    GetMovementsAsync(
        int? warehouseId,
        int? ppeProductId,
        InventoryMovementType? movementType,
        DateTime? dateFrom,
        DateTime? dateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.InventoryMovements
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.PPEProduct)
                .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(
                x => x.WarehouseId ==
                     warehouseId.Value);
        }

        if (ppeProductId.HasValue)
        {
            query = query.Where(
                x => x.PPEProductId ==
                     ppeProductId.Value);
        }

        if (movementType.HasValue)
        {
            query = query.Where(
                x => x.MovementType ==
                     movementType.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(
                x => x.CreatedAt >=
                     dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(
                x => x.CreatedAt <=
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

        return new PagedResult<InventoryMovement>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<InventoryBalance>>
    GetBalancesForUpdateAsync(
        int warehouseId,
        IReadOnlyCollection<int> productIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.InventoryBalances
            .Where(x =>
                x.WarehouseId == warehouseId &&
                productIds.Contains(x.PPEProductId))
            .OrderBy(x => x.PPEProductId)
            .ToListAsync(cancellationToken);
    }
}