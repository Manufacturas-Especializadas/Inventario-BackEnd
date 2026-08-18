using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Features.PPERequests.Delivery;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests.Commands.Deliver;

public class DeliverPPERequestCommandHandler
    : IRequestHandler<
        DeliverPPERequestCommand,
        DeliverPPERequestResultDto>
{
    private readonly IPPERequestRepository _requestRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeliverPPERequestCommandHandler(
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

    public async Task<DeliverPPERequestResultDto> Handle(
        DeliverPPERequestCommand command,
        CancellationToken cancellationToken)
    {
        var folio =
            command.Folio
                .Trim()
                .ToUpperInvariant();

        var employeeNumber =
            command.EmployeeNumber.Trim();

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
                    $"PPE request '{folio}' cannot be delivered because its current status is '{ppeRequest.Status}'.");
            }

            if (!ppeRequest.Employee.IsActive)
            {
                throw new ConflictException(
                    $"Employee '{ppeRequest.Employee.EmployeeNumber}' is inactive.");
            }

            if (!string.Equals(
                ppeRequest.Employee.EmployeeNumber,
                employeeNumber,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException(
                    $"Employee '{employeeNumber}' does not correspond to PPE request '{folio}'.");
            }

            if (!ppeRequest.Warehouse.IsActive)
            {
                throw new ConflictException(
                    $"Warehouse '{ppeRequest.Warehouse.Name}' is inactive.");
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

            // Primero validamos TODO.
            // Todavía no modificamos nada.
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
                        $"The reserved inventory for product '{item.PPEProduct.Sku}' is inconsistent. Reserved: {balance.ReservedQuantity}, required: {item.Quantity}.");
                }

                if (balance.OnHandQuantity <
                    item.Quantity)
                {
                    throw new ConflictException(
                        $"Insufficient physical inventory for product '{item.PPEProduct.Sku}'. On hand: {balance.OnHandQuantity}, required: {item.Quantity}.");
                }
            }

            var now =
                _dateTimeProvider.UtcNow;

            var movements =
                new List<InventoryMovement>();

            // Ya que todas las validaciones pasaron,
            // aplicamos los cambios.
            foreach (var item in ppeRequest.Items)
            {
                var balance =
                    balancesByProductId[
                        item.PPEProductId];

                balance.OnHandQuantity -=
                    item.Quantity;

                balance.ReservedQuantity -=
                    item.Quantity;

                movements.Add(
                    new InventoryMovement
                    {
                        WarehouseId =
                            ppeRequest.WarehouseId,

                        PPEProductId =
                            item.PPEProductId,

                        MovementType =
                            InventoryMovementType.EmployeeIssue,

                        Quantity =
                            -item.Quantity,

                        ReferenceType =
                            InventoryReferenceType.PPERequest,

                        ReferenceId =
                            ppeRequest.Id,

                        UnitCost =
                            null,

                        Reason =
                            $"PPE delivery {ppeRequest.Folio}",

                        CreatedByUserId =
                            userId,

                        CreatedAt =
                            now
                    });
            }

            ppeRequest.Status =
                PPERequestStatus.Delivered;

            ppeRequest.DeliveredByUserId =
                userId;

            ppeRequest.DeliveredAt =
                now;

            await _inventoryRepository
                .AddMovementsAsync(
                    movements,
                    cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new DeliverPPERequestResultDto
            {
                PPERequestId =
                    ppeRequest.Id,

                Folio =
                    ppeRequest.Folio,

                EmployeeNumber =
                    ppeRequest.Employee.EmployeeNumber,

                EmployeeName =
                    ppeRequest.Employee.Name,

                WarehouseId =
                    ppeRequest.WarehouseId,

                WarehouseName =
                    ppeRequest.Warehouse.Name,

                DeliveredAt =
                    now,

                Items =
                    ppeRequest.Items
                        .Select(item =>
                        {
                            var balance =
                                balancesByProductId[
                                    item.PPEProductId];

                            return new DeliveredPPEItemDto
                            {
                                PPEProductId =
                                    item.PPEProductId,

                                Sku =
                                    item.PPEProduct.Sku,

                                ProductName =
                                    item.PPEProduct.Name,

                                DeliveredQuantity =
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