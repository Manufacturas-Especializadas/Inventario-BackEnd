using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.WarehouseProducts.Commands.BulkAssign;

public class BulkAssignWarehouseProductsCommandHandler
    : IRequestHandler<
        BulkAssignWarehouseProductsCommand,
        BulkAssignWarehouseProductsResultDto>
{
    private readonly IWarehouseRepository
        _warehouseRepository;

    private readonly IPPEProductRepository
        _productRepository;

    private readonly IWarehouseProductRepository
        _warehouseProductRepository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;

    public BulkAssignWarehouseProductsCommandHandler(
        IWarehouseRepository warehouseRepository,
        IPPEProductRepository productRepository,
        IWarehouseProductRepository warehouseProductRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _warehouseRepository =
            warehouseRepository;

        _productRepository =
            productRepository;

        _warehouseProductRepository =
            warehouseProductRepository;

        _currentUser =
            currentUser;

        _dateTimeProvider =
            dateTimeProvider;
    }

    public async Task<BulkAssignWarehouseProductsResultDto>
        Handle(
            BulkAssignWarehouseProductsCommand request,
            CancellationToken cancellationToken)
    {
        var warehouseIds =
            request.WarehouseIds
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

        var productIds =
            request.PPEProductIds
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

        var warehouses =
            await _warehouseRepository
                .GetByIdsAsync(
                    warehouseIds,
                    cancellationToken);

        var missingWarehouseIds =
            warehouseIds
                .Except(
                    warehouses.Select(x => x.Id))
                .ToArray();

        if (missingWarehouseIds.Length > 0)
        {
            throw new NotFoundException(
                $"Warehouse(s) not found: {string.Join(", ", missingWarehouseIds)}.");
        }

        var inactiveWarehouses =
            warehouses
                .Where(x => !x.IsActive)
                .Select(x => x.Name)
                .ToArray();

        if (inactiveWarehouses.Length > 0)
        {
            throw new ConflictException(
                $"Inactive warehouse(s): {string.Join(", ", inactiveWarehouses)}.");
        }

        var products =
            await _productRepository
                .GetByIdsAsync(
                    productIds,
                    cancellationToken);

        var missingProductIds =
            productIds
                .Except(
                    products.Select(x => x.Id))
                .ToArray();

        if (missingProductIds.Length > 0)
        {
            throw new NotFoundException(
                $"PPE product(s) not found: {string.Join(", ", missingProductIds)}.");
        }

        var inactiveProducts =
            products
                .Where(x => !x.IsActive)
                .Select(x => x.Sku)
                .ToArray();

        if (inactiveProducts.Length > 0)
        {
            throw new ConflictException(
                $"Inactive PPE product(s): {string.Join(", ", inactiveProducts)}.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var now =
            _dateTimeProvider.UtcNow;

        var existingRelations =
            await _warehouseProductRepository
                .GetByWarehousesAndProductsAsync(
                    warehouseIds,
                    productIds,
                    cancellationToken);

        var existingByKey =
            existingRelations.ToDictionary(
                x => (
                    x.WarehouseId,
                    x.PPEProductId));

        var createdCount = 0;
        var reactivatedCount = 0;
        var alreadyActiveCount = 0;

        foreach (var warehouseId in warehouseIds)
        {
            foreach (var productId in productIds)
            {
                if (existingByKey.TryGetValue(
                    (warehouseId, productId),
                    out var relation))
                {
                    if (relation.IsActive)
                    {
                        alreadyActiveCount++;
                        continue;
                    }

                    relation.IsActive = true;
                    relation.UpdatedAt = now;
                    relation.UpdatedByUserId = userId;

                    reactivatedCount++;
                    continue;
                }

                await _warehouseProductRepository
                    .AddAsync(
                        new WarehouseProduct
                        {
                            WarehouseId = warehouseId,
                            PPEProductId = productId,
                            IsActive = true,
                            CreatedAt = now,
                            CreatedByUserId = userId
                        },
                        cancellationToken);

                createdCount++;
            }
        }

        await _warehouseProductRepository
            .SaveChangesAsync(
                cancellationToken);

        return new BulkAssignWarehouseProductsResultDto
        {
            CreatedCount = createdCount,
            ReactivatedCount = reactivatedCount,
            AlreadyActiveCount = alreadyActiveCount,
            TotalProcessed =
                createdCount +
                reactivatedCount +
                alreadyActiveCount
        };
    }
}