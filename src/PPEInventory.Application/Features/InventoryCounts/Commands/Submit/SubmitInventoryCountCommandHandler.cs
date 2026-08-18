using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.Submit;

public class SubmitInventoryCountCommandHandler
    : IRequestHandler<
        SubmitInventoryCountCommand,
        InventoryCountDto>
{
    private readonly IInventoryCountRepository _countRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SubmitInventoryCountCommandHandler(
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
        SubmitInventoryCountCommand command,
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
                InventoryCountStatus.Draft)
            {
                throw new ConflictException(
                    $"Inventory count '{folio}' cannot be submitted because its current status is '{count.Status}'.");
            }

            var uncountedItems =
                count.Items
                    .Where(x =>
                        !x.CountedQuantity.HasValue)
                    .ToArray();

            if (uncountedItems.Length > 0)
            {
                throw new ConflictException(
                    $"Inventory count '{folio}' still has {uncountedItems.Length} product(s) without a physical count.");
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

            foreach (var item in count.Items)
            {
                var systemQuantity =
                    balancesByProductId.TryGetValue(
                        item.PPEProductId,
                        out var balance)
                        ? balance.OnHandQuantity
                        : 0;

                item.SystemQuantitySnapshot =
                    systemQuantity;

                item.Variance =
                    item.CountedQuantity!.Value -
                    systemQuantity;
            }

            var now =
                _dateTimeProvider.UtcNow;

            count.Status =
                InventoryCountStatus.PendingReview;

            count.SubmittedByUserId =
                userId;

            count.SubmittedAt =
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