using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Constants;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests.Commands.Create;

public class CreatePPERequestCommandHandler
    : IRequestHandler<
        CreatePPERequestCommand,
        CreatePPERequestResultDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IRequestReasonRepository _reasonRepository;
    private readonly IPPEProductRepository _productRepository;
    private readonly IPPERequestRepository _requestRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IOrganizationalUnitRepository _organizationalUnitRepository;
    private readonly IOrganizationalUnitPPELimitRepository _organizationalUnitPPELimitRepository;
    private readonly IWarehouseProductRepository _warehouseProductRepository;

    public CreatePPERequestCommandHandler(
        IEmployeeRepository employeeRepository,
        IWarehouseRepository warehouseRepository,
        IRequestReasonRepository reasonRepository,
        IPPEProductRepository productRepository,
        IPPERequestRepository requestRepository,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider,
        IOrganizationalUnitPPELimitRepository organizationalUnitPPELimitRepository,
        IOrganizationalUnitRepository organizationalUnitRepository,
        IWarehouseProductRepository warehouseProductRepository)
    {
        _employeeRepository = employeeRepository;
        _warehouseRepository = warehouseRepository;
        _reasonRepository = reasonRepository;
        _productRepository = productRepository;
        _requestRepository = requestRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _organizationalUnitPPELimitRepository = organizationalUnitPPELimitRepository;
        _organizationalUnitRepository = organizationalUnitRepository;
        _warehouseProductRepository = warehouseProductRepository;
    }

    public async Task<CreatePPERequestResultDto> Handle(
        CreatePPERequestCommand request,
        CancellationToken cancellationToken)
    {
        var employeeNumber =
            request.EmployeeNumber.Trim();

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        await using var transaction =
            await _unitOfWork
                .BeginSerializableTransactionAsync(
                    cancellationToken);

        try
        {
            var employee =
                await _employeeRepository
                    .GetByEmployeeNumberAsync(
                        employeeNumber,
                        cancellationToken);

            if (employee is null)
            {
                throw new NotFoundException(
                    $"Employee '{employeeNumber}' was not found.");
            }

            if (!employee.IsActive)
            {
                throw new ConflictException(
                    $"Employee '{employeeNumber}' is inactive.");
            }

            var requestedForOrganizationalUnit =
    await _organizationalUnitRepository
        .GetByIdAsync(
            request.RequestedForOrganizationalUnitId,
            cancellationToken);

            if (requestedForOrganizationalUnit is null)
            {
                throw new NotFoundException(
                    $"Organizational unit with id '{request.RequestedForOrganizationalUnitId}' was not found.");
            }

            if (!requestedForOrganizationalUnit.IsActive)
            {
                throw new ConflictException(
                    $"Organizational unit '{requestedForOrganizationalUnit.Name}' is inactive.");
            }

            var organizationalPath =
                await _organizationalUnitRepository
                    .GetPathToRootAsync(
                        requestedForOrganizationalUnit.Id,
                        cancellationToken);

            if (organizationalPath.Count == 0)
            {
                throw new ConflictException(
                    $"The organizational structure for unit '{requestedForOrganizationalUnit.Name}' could not be resolved.");
            }

            var warehouse =
                await _warehouseRepository.GetByIdAsync(
                    request.WarehouseId,
                    cancellationToken);

            if (warehouse is null)
            {
                throw new NotFoundException(
                    $"Warehouse with id '{request.WarehouseId}' was not found.");
            }

            if (!warehouse.IsActive)
            {
                throw new ConflictException(
                    $"Warehouse '{warehouse.Name}' is inactive.");
            }

            var reason =
                await _reasonRepository.GetByIdAsync(
                    request.RequestReasonId,
                    cancellationToken);

            if (reason is null || !reason.IsActive)
            {
                throw new NotFoundException(
                    $"Request reason with id '{request.RequestReasonId}' was not found.");
            }

            var isExceptionalRequest =
    IsExceptionalRequestReason(
        reason.Code);

            if ( isExceptionalRequest && string.IsNullOrWhiteSpace(request.Notes) )
            {
                throw new ConflictException(
                    "Damage, Lost or Other requests require notes explaining the exceptional replacement.");
            }


            var productIds =
                request.Items
                    .Select(x => x.PPEProductId)
                    .Distinct()
                    .ToArray();

            var products =
                await _productRepository.GetByIdsAsync(
                    productIds,
                    cancellationToken);

            var productsById =
                products.ToDictionary(x => x.Id);

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

            var configuredWarehouseProducts =
    await _warehouseProductRepository
        .GetActiveByWarehouseIdAsync(
            warehouse.Id,
            cancellationToken);

            var configuredProductIds =
                configuredWarehouseProducts
                    .Select(x => x.PPEProductId)
                    .ToHashSet();

            var notConfiguredProductIds =
                productIds
                    .Where(id =>
                        !configuredProductIds.Contains(id))
                    .ToArray();

            if (notConfiguredProductIds.Length > 0)
            {
                var notConfiguredSkus =
                    notConfiguredProductIds
                        .Select(id =>
                            productsById[id].Sku)
                        .ToArray();

                throw new ConflictException(
                    $"PPE product(s) are not configured for warehouse '{warehouse.Name}': " +
                    $"{string.Join(", ", notConfiguredSkus)}.");
            }



            var organizationalUnitIds =
     organizationalPath
         .Select(x => x.Id)
         .ToArray();

            var configuredLimits =
                await _organizationalUnitPPELimitRepository
                    .GetActiveByUnitsAndProductsAsync(
                        organizationalUnitIds,
                        productIds,
                        cancellationToken);

            var limitsByUnitAndProduct =
                configuredLimits.ToDictionary(
                    x => (
                        x.OrganizationalUnitId,
                        x.PPEProductId
                    ));

            var appliedMaxByProductId =
                new Dictionary<int, int?>();

            var now =
                _dateTimeProvider.UtcNow;

            var warnings = new List<PPERequestWarningDto>();

            var committedQuantitiesByProductId =
                isExceptionalRequest
                    ? new Dictionary<int, int>()
                    : await _requestRepository
                        .GetCommittedNormalQuantitiesInCycleAsync(
                            requestedForOrganizationalUnit.Id,
                            productIds,
                            now,
                            cancellationToken);

            var balances =
                await _inventoryRepository
                    .GetBalancesForUpdateAsync(
                        warehouse.Id,
                        productIds,
                        cancellationToken);

            var balancesByProductId =
                balances.ToDictionary(
                    x => x.PPEProductId);

            var productsWithoutBalance =
                productIds
                    .Where(id =>
                        !balancesByProductId
                            .ContainsKey(id))
                                .ToArray();

            if (productsWithoutBalance.Length > 0)
            {
                throw new ConflictException(
                    $"The following PPE product(s) are not available in warehouse '{warehouse.Name}': " +
                    $"{string.Join(", ", productsWithoutBalance)}.");
            }

            foreach (var requestItem in request.Items)
            {
                var product =
                    productsById[
                        requestItem.PPEProductId];

                int? effectiveMax = null;

                foreach (var unit in organizationalPath)
                {
                    if (
                        limitsByUnitAndProduct.TryGetValue(
                            (
                                unit.Id,
                                product.Id
                            ),
                            out var configuredLimit)
                    )
                    {
                        effectiveMax =
    configuredLimit
        .MaxQuantityPerCycle;

                        break;
                    }
                }

                effectiveMax ??=
    product.DefaultMaxQuantityPerCycle;

                appliedMaxByProductId[
                    product.Id
                ] = effectiveMax;

                /*
                 * Si no existe máximo configurado,
                 * no hay límite de ciclo para este producto.
                 */
                if (!effectiveMax.HasValue)
                {
                    continue;
                }

                /*
                 * Si existe máximo, necesitamos una vida útil
                 * para poder saber cuándo se libera el cupo.
                 */
                if (
                    !product.ReplacementIntervalDays.HasValue ||
                    product.ReplacementIntervalDays.Value <= 0
                )
                {
                    throw new ConflictException(
                        $"Product '{product.Sku}' has a quantity limit but does not have a valid replacement interval configured.");
                }

                /*
                 * Damage / Lost / Other son excepciones.
                 * No consumen ni son bloqueadas por el cupo normal.
                 */
                if (isExceptionalRequest)
                {
                    continue;
                }

                committedQuantitiesByProductId.TryGetValue(
                    product.Id,
                    out var committedQuantity);

                var projectedQuantity =
                    committedQuantity +
                    requestItem.Quantity;

                if (
                    projectedQuantity >
                    effectiveMax.Value
                )
                {
                    var remainingQuantity =
                        Math.Max(
                            0,
                            effectiveMax.Value -
                            committedQuantity);

                    throw new ConflictException(
                        $"Product '{product.Sku}' has reached the quantity limit for organizational unit '{requestedForOrganizationalUnit.Name}'. " +
                        $"Maximum per replacement cycle: {effectiveMax.Value}. " +
                        $"Currently committed: {committedQuantity}. " +
                        $"Available to request: {remainingQuantity}. " +
                        $"Requested: {requestItem.Quantity}.");
                }
            }

            foreach (var requestItem in request.Items)
            {
                var product =
                    productsById[
                        requestItem.PPEProductId];

                if (
                    !balancesByProductId.TryGetValue(
                        requestItem.PPEProductId,
                        out var balance)
                )
                {
                    throw new ConflictException(
                        $"Product '{product.Sku}' has no inventory available in warehouse '{warehouse.Name}'.");
                }

                var availableQuantity =
                    balance.OnHandQuantity -
                    balance.ReservedQuantity;

                if (
                    availableQuantity <
                    requestItem.Quantity
                )
                {
                    throw new ConflictException(
                        $"Product '{product.Sku}' does not have enough available inventory in warehouse '{warehouse.Name}'. " +
                        $"Available: {availableQuantity}. " +
                        $"Requested: {requestItem.Quantity}.");
                }
            }

            var ppeRequest =
                new PPERequest
                {
                    EmployeeId =
                        employee.Id,

                    RequestedForOrganizationalUnitId =
    requestedForOrganizationalUnit.Id,

                    WarehouseId =
                        warehouse.Id,

                    RequestReasonId =
                        reason.Id,

                    Status =
                        PPERequestStatus.Pending,

                    Notes =
                        Normalize(request.Notes),

                    CreatedByUserId =
                        userId,

                    CreatedAt =
                        now
                };

            foreach (var requestItem in request.Items)
            {
                ppeRequest.Items.Add(
                    new PPERequestItem
                    {
                        PPEProductId =
                            requestItem.PPEProductId,

                        Quantity =
                            requestItem.Quantity,

                        AppliedMaxQuantityPerCycle =
                            appliedMaxByProductId[
                                requestItem.PPEProductId]
                    });

                var balance =
                    balancesByProductId[
                        requestItem.PPEProductId];

                var product =
                    productsById[
                        requestItem.PPEProductId];

                var availableQuantity =
                    balance.OnHandQuantity -
                    balance.ReservedQuantity;

                if (
                    requestItem.Quantity >
                    availableQuantity
                )
                {
                    throw new ConflictException(
                        $"Product '{product.Sku}' does not have enough available stock in warehouse '{warehouse.Name}'. " +
                        $"Available: {availableQuantity}. " +
                        $"Requested: {requestItem.Quantity}.");
                }

                balance.ReservedQuantity +=
                    requestItem.Quantity;
            }

            await _requestRepository.AddAsync(
                ppeRequest,
                cancellationToken);

            // Un solo SaveChanges:
            // Request + Items + ReservedQuantity.
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new CreatePPERequestResultDto
            {
                Request =
                    MapRequest(
                        ppeRequest,
                        employee,
                        requestedForOrganizationalUnit,
                        warehouse,
                        reason,
                        productsById),

                Warnings =
                    warnings
            };
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    private static bool IsExceptionalRequestReason(
        string code)
    {
        return code is
            RequestReasonCodes.Damage or
            RequestReasonCodes.Lost or
            RequestReasonCodes.Other;
    }

    private static PPERequestDto MapRequest(
        PPERequest request,
        Employee employee,
        OrganizationalUnit organizationalUnit,
        Warehouse warehouse,
        RequestReason reason,
        IReadOnlyDictionary<int, PPEProduct> products)
    {
        return new PPERequestDto
        {
            Id = request.Id,

            Folio =
                request.Folio,

            Status =
                request.Status,

            EmployeeId =
                employee.Id,

            EmployeeNumber =
                employee.EmployeeNumber,

            EmployeeName =
                employee.Name,
            RequestedForOrganizationalUnitId =
    organizationalUnit.Id,

            RequestedForOrganizationalUnitName =
    organizationalUnit.Name,

            WarehouseId =
                warehouse.Id,

            WarehouseName =
                warehouse.Name,

            RequestReasonId =
                reason.Id,

            RequestReason =
                reason.Name,

            Notes =
                request.Notes,

            CreatedAt =
                request.CreatedAt,

            Items =
                request.Items
                    .Select(item =>
                    {
                        var product =
                            products[
                                item.PPEProductId];

                        return new PPERequestItemDto
                        {
                            PPEProductId =
                                product.Id,

                            Sku =
                                product.Sku,

                            ProductName =
                                product.Name,

                            Quantity =
                                item.Quantity,

                            ReplacementIntervalDays =
                                product.ReplacementIntervalDays
                        };
                    })
                    .ToArray()
        };
    }

    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
