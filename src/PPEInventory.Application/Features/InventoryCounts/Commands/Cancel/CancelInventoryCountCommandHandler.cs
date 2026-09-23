using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features
    .InventoryCounts.Commands.Cancel;

public class CancelInventoryCountCommandHandler
    : IRequestHandler<
        CancelInventoryCountCommand,
        InventoryCountDto>
{
    private readonly
        IInventoryCountRepository
        _countRepository;

    private readonly
        IUnitOfWork
        _unitOfWork;

    private readonly
        ICurrentUserService
        _currentUser;

    private readonly
        IDateTimeProvider
        _dateTimeProvider;

    public CancelInventoryCountCommandHandler(
        IInventoryCountRepository countRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _countRepository =
            countRepository;

        _unitOfWork =
            unitOfWork;

        _currentUser =
            currentUser;

        _dateTimeProvider =
            dateTimeProvider;
    }

    public async Task<InventoryCountDto> Handle(
        CancelInventoryCountCommand command,
        CancellationToken cancellationToken)
    {
        var folio =
            command.Folio
                .Trim()
                .ToUpperInvariant();

        var reason =
            command.Reason.Trim();

        var userId =
            _currentUser.UserId
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
                    $"Inventory count '{folio}' cannot be cancelled because its current status is '{count.Status}'.");
            }

            var now =
                _dateTimeProvider.UtcNow;

            count.Status =
                InventoryCountStatus.Cancelled;

            count.CancelledByUserId =
                userId;

            count.CancelledAt =
                now;

            count.CancellationReason =
                reason;

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