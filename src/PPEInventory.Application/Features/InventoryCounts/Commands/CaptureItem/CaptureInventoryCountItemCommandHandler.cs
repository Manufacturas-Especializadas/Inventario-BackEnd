using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.CaptureItem;

public class CaptureInventoryCountItemCommandHandler
    : IRequestHandler<
        CaptureInventoryCountItemCommand,
        InventoryCountItemDto>
{
    private readonly IInventoryCountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CaptureInventoryCountItemCommandHandler(
        IInventoryCountRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<InventoryCountItemDto> Handle(
        CaptureInventoryCountItemCommand command,
        CancellationToken cancellationToken)
    {
        var folio =
            command.Folio.Trim().ToUpperInvariant();

        var count =
            await _repository.GetByFolioForUpdateAsync(
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
                $"Inventory count '{folio}' cannot be modified because its current status is '{count.Status}'.");
        }

        var item =
            count.Items.FirstOrDefault(
                x =>
                    x.PPEProductId ==
                    command.PPEProductId);

        if (item is null)
        {
            throw new NotFoundException(
                $"Product with id '{command.PPEProductId}' is not part of inventory count '{folio}'.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        item.CountedQuantity =
            command.CountedQuantity;

        item.CountedByUserId =
            userId;

        item.CountedAt =
            _dateTimeProvider.UtcNow;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new InventoryCountItemDto
        {
            Id = item.Id,

            PPEProductId =
                item.PPEProductId,

            Sku =
                item.PPEProduct.Sku,

            ProductName =
                item.PPEProduct.Name,

            CategoryName =
                item.PPEProduct.Category.Name,

            CountedQuantity =
                item.CountedQuantity,

            // Blind count
            SystemQuantity = null,
            Variance = null,

            CountedAt =
                item.CountedAt
        };
    }
}