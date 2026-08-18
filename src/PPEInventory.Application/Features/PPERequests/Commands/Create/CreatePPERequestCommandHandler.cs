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

    public CreatePPERequestCommandHandler(
        IEmployeeRepository employeeRepository,
        IWarehouseRepository warehouseRepository,
        IRequestReasonRepository reasonRepository,
        IPPEProductRepository productRepository,
        IPPERequestRepository requestRepository,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
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

            // Validar máximo por solicitud.
            foreach (var requestItem in request.Items)
            {
                var product =
                    productsById[
                        requestItem.PPEProductId];

                if (product.MaxQuantityPerRequest.HasValue &&
                    requestItem.Quantity >
                    product.MaxQuantityPerRequest.Value)
                {
                    throw new ConflictException(
                        $"Product '{product.Sku}' allows a maximum of {product.MaxQuantityPerRequest.Value} unit(s) per request.");
                }
            }

            var now =
                _dateTimeProvider.UtcNow;

            var warnings =
                new List<PPERequestWarningDto>();

            foreach (var requestItem in request.Items)
            {
                var product =
                    productsById[
                        requestItem.PPEProductId];

                if (!product.ReplacementIntervalDays.HasValue)
                {
                    continue;
                }

                var lastDeliveredAt =
                    await _requestRepository
                        .GetLastDeliveredAtAsync(
                            employee.Id,
                            product.Id,
                            cancellationToken);

                if (!lastDeliveredAt.HasValue)
                {
                    continue;
                }

                var nextEligibleDate =
                    lastDeliveredAt.Value.Date
                        .AddDays(
                            product.ReplacementIntervalDays.Value);

                if (now.Date >= nextEligibleDate)
                {
                    continue;
                }

                warnings.Add(
                    new PPERequestWarningDto
                    {
                        PPEProductId =
                            product.Id,

                        Sku =
                            product.Sku,

                        ProductName =
                            product.Name,

                        LastDeliveredAt =
                            lastDeliveredAt.Value,

                        NextEligibleDate =
                            nextEligibleDate,

                        Message =
                            $"Product '{product.Sku}' is being requested before its replacement interval has expired."
                    });
            }

            // Si hay reemplazo anticipado,
            // debe existir una justificación especial.
            if (warnings.Count > 0 &&
                !IsAllowedEarlyReplacementReason(
                    reason.Code))
            {
                throw new ConflictException(
                    "Early replacement detected. Select Damage, Lost or Other as the request reason to continue.");
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

            foreach (var requestItem in request.Items)
            {
                var product =
                    productsById[
                        requestItem.PPEProductId];

                if (!balancesByProductId.TryGetValue(
                    product.Id,
                    out var balance))
                {
                    throw new ConflictException(
                        $"Product '{product.Sku}' has no inventory in warehouse '{warehouse.Name}'.");
                }

                var available =
                    balance.OnHandQuantity -
                    balance.ReservedQuantity;

                if (available < requestItem.Quantity)
                {
                    throw new ConflictException(
                        $"Insufficient available inventory for product '{product.Sku}'. Available: {available}, requested: {requestItem.Quantity}.");
                }
            }

            var ppeRequest =
                new PPERequest
                {
                    EmployeeId =
                        employee.Id,

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
                            requestItem.Quantity
                    });

                var balance =
                    balancesByProductId[
                        requestItem.PPEProductId];

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

    private static bool IsAllowedEarlyReplacementReason(
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
