using System.Text.Json;
using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Constants;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.InventoryAdjustments.Commands.Create;

public class CreateInventoryAdjustmentCommandHandler
    : IRequestHandler<
        CreateInventoryAdjustmentCommand,
        InventoryAdjustmentDto>
{
    private readonly IInventoryAdjustmentRepository
        _adjustmentRepository;

    private readonly IWarehouseRepository
        _warehouseRepository;

    private readonly IPPEProductRepository
        _productRepository;

    private readonly IInventoryRepository
        _inventoryRepository;

    private readonly IAuditLogRepository
        _auditLogRepository;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;

    public CreateInventoryAdjustmentCommandHandler(
        IInventoryAdjustmentRepository adjustmentRepository,
        IWarehouseRepository warehouseRepository,
        IPPEProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _adjustmentRepository = adjustmentRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<InventoryAdjustmentDto> Handle(
        CreateInventoryAdjustmentCommand command,
        CancellationToken cancellationToken)
    {
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
            var warehouse =
                await _warehouseRepository.GetByIdAsync(
                    command.WarehouseId,
                    cancellationToken);

            if (warehouse is null)
            {
                throw new NotFoundException(
                    $"Warehouse with id '{command.WarehouseId}' was not found.");
            }

            if (!warehouse.IsActive)
            {
                throw new ConflictException(
                    $"Warehouse '{warehouse.Name}' is inactive.");
            }

            var productIds =
                command.Items
                    .Select(x => x.PPEProductId)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();

            var products =
                await _productRepository.GetByIdsAsync(
                    productIds,
                    cancellationToken);

            var productsById =
                products.ToDictionary(
                    x => x.Id);

            var missingProductIds =
                productIds
                    .Where(id =>
                        !productsById.ContainsKey(id))
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

            var balances =
                await _inventoryRepository
                    .GetBalancesForUpdateAsync(
                        warehouse.Id,
                        productIds,
                        cancellationToken);

            var balancesByProductId =
                balances.ToDictionary(
                    x => x.PPEProductId);

            var plans =
                new List<AdjustmentPlan>();

            // Primero validamos todo sin modificar inventario.
            foreach (var requestItem in command.Items)
            {
                var product =
                    productsById[
                        requestItem.PPEProductId];

                balancesByProductId.TryGetValue(
                    product.Id,
                    out var balance);

                var previousOnHand =
                    balance?.OnHandQuantity ?? 0;

                var reserved =
                    balance?.ReservedQuantity ?? 0;

                if (balance is null &&
                    requestItem.QuantityAdjustment < 0)
                {
                    throw new ConflictException(
                        $"Product '{product.Sku}' has no inventory balance and cannot receive a negative adjustment.");
                }

                var candidate =
                    (long)previousOnHand +
                    requestItem.QuantityAdjustment;

                if (candidate < 0)
                {
                    throw new ConflictException(
                        $"Adjustment would make OnHand negative for product '{product.Sku}'. Current: {previousOnHand}, adjustment: {requestItem.QuantityAdjustment}.");
                }

                if (candidate > int.MaxValue)
                {
                    throw new ConflictException(
                        $"Adjustment exceeds the supported inventory quantity for product '{product.Sku}'.");
                }

                var newOnHand =
                    (int)candidate;

                if (newOnHand < reserved)
                {
                    throw new ConflictException(
                        $"Adjustment cannot be applied to product '{product.Sku}' because new OnHand ({newOnHand}) would be lower than Reserved ({reserved}).");
                }

                plans.Add(
                    new AdjustmentPlan(
                        product,
                        balance,
                        requestItem.QuantityAdjustment,
                        previousOnHand,
                        reserved,
                        newOnHand));
            }

            var now =
                _dateTimeProvider.UtcNow;

            var adjustment =
                new InventoryAdjustment
                {
                    WarehouseId =
                        warehouse.Id,

                    Reason =
                        reason,

                    CreatedByUserId =
                        userId,

                    CreatedAt =
                        now
                };

            // Ya pasó toda la validación.
            foreach (var plan in plans)
            {
                if (plan.Balance is null)
                {
                    var newBalance =
                        new InventoryBalance
                        {
                            WarehouseId =
                                warehouse.Id,

                            PPEProductId =
                                plan.Product.Id,

                            OnHandQuantity =
                                plan.NewOnHand,

                            ReservedQuantity =
                                0
                        };

                    await _inventoryRepository
                        .AddBalanceAsync(
                            newBalance,
                            cancellationToken);
                }
                else
                {
                    plan.Balance.OnHandQuantity =
                        plan.NewOnHand;
                }

                adjustment.Items.Add(
                    new InventoryAdjustmentItem
                    {
                        PPEProductId =
                            plan.Product.Id,

                        QuantityAdjustment =
                            plan.QuantityAdjustment,

                        PreviousOnHandQuantity =
                            plan.PreviousOnHand,

                        NewOnHandQuantity =
                            plan.NewOnHand,

                        ReservedQuantitySnapshot =
                            plan.ReservedQuantity
                    });
            }

            await _adjustmentRepository.AddAsync(
                adjustment,
                cancellationToken);

            // Genera Adjustment.Id y Folio.
            // También actualiza balances, pero seguimos
            // dentro de la misma transacción.
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            var movements =
                plans
                    .Select(plan =>
                        new InventoryMovement
                        {
                            WarehouseId =
                                warehouse.Id,

                            PPEProductId =
                                plan.Product.Id,

                            MovementType =
                                plan.QuantityAdjustment > 0
                                    ? InventoryMovementType
                                        .AdjustmentIncrease
                                    : InventoryMovementType
                                        .AdjustmentDecrease,

                            Quantity =
                                plan.QuantityAdjustment,

                            ReferenceType =
                                InventoryReferenceType
                                    .ManualAdjustment,

                            ReferenceId =
                                adjustment.Id,

                            UnitCost =
                                null,

                            Reason =
                                $"Manual adjustment {adjustment.Folio}: {reason}",

                            CreatedByUserId =
                                userId,

                            CreatedAt =
                                now
                        })
                    .ToArray();

            await _inventoryRepository.AddMovementsAsync(
                movements,
                cancellationToken);

            var oldValues =
                plans.Select(plan =>
                    new
                    {
                        ppeProductId =
                            plan.Product.Id,

                        sku =
                            plan.Product.Sku,

                        onHandQuantity =
                            plan.PreviousOnHand,

                        reservedQuantity =
                            plan.ReservedQuantity,

                        availableQuantity =
                            plan.PreviousOnHand -
                            plan.ReservedQuantity
                    });

            var newValues =
                plans.Select(plan =>
                    new
                    {
                        ppeProductId =
                            plan.Product.Id,

                        sku =
                            plan.Product.Sku,

                        adjustment =
                            plan.QuantityAdjustment,

                        onHandQuantity =
                            plan.NewOnHand,

                        reservedQuantity =
                            plan.ReservedQuantity,

                        availableQuantity =
                            plan.NewOnHand -
                            plan.ReservedQuantity
                    });

            var auditLog =
                new AuditLog
                {
                    EntityName =
                        nameof(InventoryAdjustment),

                    EntityId =
                        adjustment.Id.ToString(),

                    Action =
                        AuditActions
                            .InventoryAdjustmentPosted,

                    Description =
                        $"Inventory adjustment {adjustment.Folio}. Reason: {reason}",

                    OldValuesJson =
                        JsonSerializer.Serialize(
                            oldValues),

                    NewValuesJson =
                        JsonSerializer.Serialize(
                            newValues),

                    PerformedByUserId =
                        userId,

                    CreatedAt =
                        now
                };

            await _auditLogRepository.AddAsync(
                auditLog,
                cancellationToken);

            // Movements + AuditLog.
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new InventoryAdjustmentDto
            {
                Id =
                    adjustment.Id,

                Folio =
                    adjustment.Folio,

                WarehouseId =
                    warehouse.Id,

                WarehouseCode =
                    warehouse.Code,

                WarehouseName =
                    warehouse.Name,

                Reason =
                    adjustment.Reason,

                CreatedByUserId =
                    userId,

                CreatedAt =
                    now,

                Items =
                    plans.Select(plan =>
                        new InventoryAdjustmentItemDto
                        {
                            PPEProductId =
                                plan.Product.Id,

                            Sku =
                                plan.Product.Sku,

                            ProductName =
                                plan.Product.Name,

                            QuantityAdjustment =
                                plan.QuantityAdjustment,

                            PreviousOnHandQuantity =
                                plan.PreviousOnHand,

                            NewOnHandQuantity =
                                plan.NewOnHand,

                            ReservedQuantity =
                                plan.ReservedQuantity,

                            AvailableQuantity =
                                plan.NewOnHand -
                                plan.ReservedQuantity
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

    private sealed record AdjustmentPlan(
        PPEProduct Product,
        InventoryBalance? Balance,
        int QuantityAdjustment,
        int PreviousOnHand,
        int ReservedQuantity,
        int NewOnHand);
}