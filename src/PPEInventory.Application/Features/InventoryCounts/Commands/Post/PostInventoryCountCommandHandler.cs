using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.Post;

public class PostInventoryCountCommandHandler
    : IRequestHandler<
        PostInventoryCountCommand,
        InventoryCountDto>
{
    private readonly IInventoryCountRepository _countRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PostInventoryCountCommandHandler(
        IInventoryCountRepository countRepository,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _countRepository = countRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<InventoryCountDto> Handle(
        PostInventoryCountCommand command,
        CancellationToken cancellationToken)
    {
        var folio =
            command.Folio.Trim().ToUpperInvariant();

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        await using var transaction =
            await _unitOfWork
                .BeginSerializableTransactionAsync(
                    cancellationToken);

        try
        {
            var count =
                await _countRepository
                    .GetByFolioForUpdateAsync(
                        folio,
                        cancellationToken);

            if (count is null)
            {
                throw new NotFoundException(
                    $"Inventory count '{folio}' was not found.");
            }

            if (count.Status !=
                InventoryCountStatus.PendingReview)
            {
                throw new ConflictException(
                    $"Inventory count '{folio}' cannot be posted because its current status is '{count.Status}'.");
            }

            var productIds =
                count.Items
                    .Select(x => x.PPEProductId)
                    .OrderBy(x => x)
                    .ToArray();

            var balances =
                await _inventoryRepository
                    .GetBalancesForUpdateAsync(
                        count.WarehouseId,
                        productIds,
                        cancellationToken);

            var balancesByProductId =
                balances.ToDictionary(
                    x => x.PPEProductId);

            var now =
                _dateTimeProvider.UtcNow;

            var movements =
                new List<InventoryMovement>();

            foreach (var item in
                count.Items.OrderBy(
                    x => x.PPEProductId))
            {
                if (!item.SystemQuantitySnapshot.HasValue ||
                    !item.CountedQuantity.HasValue ||
                    !item.Variance.HasValue)
                {
                    throw new ConflictException(
                        $"Inventory count item for product '{item.PPEProduct.Sku}' does not contain a valid submitted snapshot.");
                }

                var variance =
                    item.Variance.Value;

                if (variance == 0)
                {
                    continue;
                }

                InventoryBalance balance;

                if (!balancesByProductId.TryGetValue(
                    item.PPEProductId,
                    out balance!))
                {
                    // Si al momento del snapshot era diferente de cero,
                    // debería existir un balance.
                    if (item.SystemQuantitySnapshot.Value != 0)
                    {
                        throw new ConflictException(
                            $"Inventory balance for product '{item.PPEProduct.Sku}' is inconsistent with the submitted count.");
                    }

                    balance =
                        new InventoryBalance
                        {
                            WarehouseId =
                                count.WarehouseId,

                            PPEProductId =
                                item.PPEProductId,

                            OnHandQuantity =
                                0,

                            ReservedQuantity =
                                0
                        };

                    await _inventoryRepository
                        .AddBalanceAsync(
                            balance,
                            cancellationToken);

                    balancesByProductId[
                        item.PPEProductId] =
                        balance;
                }

                var newOnHand =
                    balance.OnHandQuantity +
                    variance;

                if (newOnHand < 0)
                {
                    throw new ConflictException(
                        $"Posting inventory count '{folio}' would make OnHand negative for product '{item.PPEProduct.Sku}'.");
                }

                if (newOnHand <
                    balance.ReservedQuantity)
                {
                    throw new ConflictException(
                        $"Inventory count '{folio}' cannot be posted for product '{item.PPEProduct.Sku}' because the adjusted OnHand quantity ({newOnHand}) would be lower than the currently reserved quantity ({balance.ReservedQuantity}).");
                }

                balance.OnHandQuantity =
                    newOnHand;

                movements.Add(
                    new InventoryMovement
                    {
                        WarehouseId =
                            count.WarehouseId,

                        PPEProductId =
                            item.PPEProductId,

                        MovementType =
                            InventoryMovementType
                                .CountAdjustment,

                        Quantity =
                            variance,

                        ReferenceType =
                            InventoryReferenceType
                                .InventoryCount,

                        ReferenceId =
                            count.Id,

                        UnitCost =
                            null,

                        Reason =
                            $"Inventory count adjustment {count.Folio}",

                        CreatedByUserId =
                            userId,

                        CreatedAt =
                            now
                    });
            }

            if (movements.Count > 0)
            {
                await _inventoryRepository
                    .AddMovementsAsync(
                        movements,
                        cancellationToken);
            }

            count.Status =
                InventoryCountStatus.Posted;

            count.PostedByUserId =
                userId;

            count.PostedAt =
                now;

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return count.ToDto();
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}