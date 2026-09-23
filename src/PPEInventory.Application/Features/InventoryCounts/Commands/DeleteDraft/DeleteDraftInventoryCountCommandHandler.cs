using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features
    .InventoryCounts.Commands.DeleteDraft;

public class DeleteDraftInventoryCountCommandHandler
    : IRequestHandler<
        DeleteDraftInventoryCountCommand>
{
    private readonly
        IInventoryCountRepository
        _countRepository;

    private readonly
        IUnitOfWork
        _unitOfWork;

    public DeleteDraftInventoryCountCommandHandler(
        IInventoryCountRepository countRepository,
        IUnitOfWork unitOfWork)
    {
        _countRepository =
            countRepository;

        _unitOfWork =
            unitOfWork;
    }

    public async Task Handle(
        DeleteDraftInventoryCountCommand command,
        CancellationToken cancellationToken)
    {
        var folio =
            command.Folio
                .Trim()
                .ToUpperInvariant();

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
                    $"Inventory count '{folio}' cannot be deleted because its current status is '{count.Status}'.");
            }

            _countRepository.Remove(
                count);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}