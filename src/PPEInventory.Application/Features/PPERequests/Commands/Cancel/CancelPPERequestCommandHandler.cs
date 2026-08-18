using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Features.PPERequests.Cancellation;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests.Commands.Cancel;

public class CancelPPERequestCommandHandler
    : IRequestHandler<
        CancelPPERequestCommand,
        CancelPPERequestResultDto>
{
    private readonly IPPERequestRepository _requestRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelPPERequestCommandHandler(
        IPPERequestRepository requestRepository,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _requestRepository = requestRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<CancelPPERequestResultDto> Handle(
        CancelPPERequestCommand command,
        CancellationToken cancellationToken)
    {
        var folio =
            command.Folio
                .Trim()
                .ToUpperInvariant();

        var cancellationReason =
            command.CancellationReason.Trim();

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        await using var transaction =
            await _unitOfWork
                .BeginSerializableTransactionAsync(
                    cancellationToken);

        try
        {
            var ppeRequest =
                await _requestRepository
                    .GetByFolioForUpdateAsync(
                        folio,
                        cancellationToken);

            if (ppeRequest is null)
            {
                throw new NotFoundException(
                    $"PPE request '{folio}' was not found.");
            }

            if (ppeRequest.Status !=
                PPERequestStatus.Pending)
            {
                throw new ConflictException(
                    $"PPE request '{folio}' cannot be cancelled because its current status is '{ppeRequest.Status}'.");
            }

            if (ppeRequest.Items.Count == 0)
            {
                throw new ConflictException(
                    $"PPE request '{folio}' does not contain items.");
            }

            var productIds =
                ppeRequest.Items
                    .Select(x => x.PPEProductId)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();

            var balances =
                await _inventoryRepository
                    .GetBalancesForUpdateAsync(
                        ppeRequest.WarehouseId,
                        productIds,
                        cancellationToken);

            var balancesByProductId =
                balances.ToDictionary(
                    x => x.PPEProductId);

            // Primero validamos todos los balances.
            foreach (var item in ppeRequest.Items)
            {
                if (!balancesByProductId.TryGetValue(
                    item.PPEProductId,
                    out var balance))
                {
                    throw new ConflictException(
                        $"Inventory balance was not found for product '{item.PPEProduct.Sku}'.");
                }

                if (balance.ReservedQuantity <
                    item.Quantity)
                {
                    throw new ConflictException(
                        $"Reserved inventory for product '{item.PPEProduct.Sku}' is inconsistent. Reserved: {balance.ReservedQuantity}, required to release: {item.Quantity}.");
                }
            }

            // Ahora sí liberamos reservas.
            foreach (var item in ppeRequest.Items)
            {
                var balance =
                    balancesByProductId[
                        item.PPEProductId];

                balance.ReservedQuantity -=
                    item.Quantity;
            }

            var now =
                _dateTimeProvider.UtcNow;

            ppeRequest.Status =
                PPERequestStatus.Cancelled;

            ppeRequest.CancelledByUserId =
                userId;

            ppeRequest.CancelledAt =
                now;

            ppeRequest.CancellationReason =
                cancellationReason;

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new CancelPPERequestResultDto
            {
                PPERequestId =
                    ppeRequest.Id,

                Folio =
                    ppeRequest.Folio,

                EmployeeNumber =
                    ppeRequest.Employee.EmployeeNumber,

                EmployeeName =
                    ppeRequest.Employee.Name,

                CancelledAt =
                    now,

                CancellationReason =
                    cancellationReason,

                Items =
                    ppeRequest.Items
                        .Select(item =>
                        {
                            var balance =
                                balancesByProductId[
                                    item.PPEProductId];

                            return new CancelledPPEItemDto
                            {
                                PPEProductId =
                                    item.PPEProductId,

                                Sku =
                                    item.PPEProduct.Sku,

                                ProductName =
                                    item.PPEProduct.Name,

                                ReleasedQuantity =
                                    item.Quantity,

                                OnHandQuantity =
                                    balance.OnHandQuantity,

                                ReservedQuantity =
                                    balance.ReservedQuantity,

                                AvailableQuantity =
                                    balance.OnHandQuantity -
                                    balance.ReservedQuantity
                            };
                        })
                        .ToArray()
            };
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}