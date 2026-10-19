using System.Text.Json;
using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Constants;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests.Commands.Approve;

public class ApprovePPERequestCommandHandler
    : IRequestHandler<ApprovePPERequestCommand, ApprovePPERequestResultDto>
{
    private readonly IPPERequestRepository _requestRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IWarehouseProductRepository _warehouseProductRepository;
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApprovePPERequestCommandHandler(
        IPPERequestRepository requestRepository,
        IInventoryRepository inventoryRepository,
        IWarehouseProductRepository warehouseProductRepository,
        IInventoryCountRepository inventoryCountRepository,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _requestRepository = requestRepository;
        _inventoryRepository = inventoryRepository;
        _warehouseProductRepository = warehouseProductRepository;
        _inventoryCountRepository = inventoryCountRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ApprovePPERequestResultDto> Handle(
        ApprovePPERequestCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Authenticated user was not found.");
        var folio = command.Folio.Trim().ToUpperInvariant();

        await using var transaction = await _unitOfWork.BeginSerializableTransactionAsync(cancellationToken);
        try
        {
            var ppeRequest = await _requestRepository.GetByFolioForUpdateAsync(folio, cancellationToken)
                ?? throw new NotFoundException($"PPE request '{folio}' was not found.");
            if (ppeRequest.Status != PPERequestStatus.PendingApproval)
                throw new ConflictException($"PPE request '{folio}' cannot be approved because its current status is '{ppeRequest.Status}'.");
            if (ppeRequest.RequestReason is null ||
                ppeRequest.RequestReason.Code is not
                    (RequestReasonCodes.Damage or RequestReasonCodes.Lost or RequestReasonCodes.Other))
                throw new ConflictException("Only exceptional PPE requests can be approved.");

            if (string.IsNullOrWhiteSpace(ppeRequest.Notes))
                throw new ConflictException("Exceptional PPE requests require notes.");
            if (ppeRequest.Employee is null || !ppeRequest.Employee.IsActive)
                throw new ConflictException("The requesting employee is missing or inactive.");
            if (!ppeRequest.RequestedForOrganizationalUnitId.HasValue ||
                ppeRequest.RequestedForOrganizationalUnit is null ||
                !ppeRequest.RequestedForOrganizationalUnit.IsActive)
                throw new ConflictException("The requested organizational unit is missing or inactive.");
            if (ppeRequest.Warehouse is null || !ppeRequest.Warehouse.IsActive)
                throw new ConflictException("The requested warehouse is missing or inactive.");
            if (ppeRequest.Items.Count == 0)
                throw new ConflictException("The PPE request does not contain items.");
            if (ppeRequest.Items.Any(x => x.Quantity <= 0) ||
                ppeRequest.Items.Select(x => x.PPEProductId).Distinct().Count() != ppeRequest.Items.Count)
                throw new ConflictException("PPE request items must have positive quantities and unique products.");
            if (ppeRequest.Items.Any(x => x.PPEProduct is null || !x.PPEProduct.IsActive))
                throw new ConflictException("All requested products must exist and be active.");

            var productIds = ppeRequest.Items.Select(x => x.PPEProductId).ToArray();
            var configurations = await _warehouseProductRepository.GetByWarehousesAndProductsAsync(
                new[] { ppeRequest.WarehouseId }, productIds, cancellationToken);
            var activeProductIds = configurations.Where(x => x.IsActive)
                .Select(x => x.PPEProductId).ToHashSet();
            if (productIds.Any(id => !activeProductIds.Contains(id)))
                throw new ConflictException("All requested products must have an active warehouse configuration.");

            if (await _inventoryCountRepository.HasDraftCountAsync(
                ppeRequest.WarehouseId, cancellationToken))
                throw new ConflictException("The warehouse has a draft inventory count and cannot reserve inventory.");

            // The repository orders balances by PPEProductId within this serializable transaction.
            var balances = await _inventoryRepository.GetBalancesForUpdateAsync(
                ppeRequest.WarehouseId, productIds, cancellationToken);
            var balancesByProductId = balances.ToDictionary(x => x.PPEProductId);

            // Validate every item before modifying any balance.
            foreach (var item in ppeRequest.Items)
            {
                if (!balancesByProductId.TryGetValue(item.PPEProductId, out var balance))
                    throw new ConflictException($"Inventory balance was not found for product '{item.PPEProduct.Sku}'.");
                if (balance.OnHandQuantity - balance.ReservedQuantity < item.Quantity)
                    throw new ConflictException($"Insufficient available inventory for product '{item.PPEProduct.Sku}'.");
            }

            foreach (var item in ppeRequest.Items)
                balancesByProductId[item.PPEProductId].ReservedQuantity += item.Quantity;

            var oldValues = JsonSerializer.Serialize(new
            {
                Status = ppeRequest.Status.ToString(),
                ppeRequest.ApprovedByUserId,
                ppeRequest.ApprovedAt,
                ppeRequest.RejectedByUserId,
                ppeRequest.RejectedAt,
                ppeRequest.RejectionReason
            });
            var now = _dateTimeProvider.UtcNow;
            ppeRequest.Status = PPERequestStatus.Pending;
            ppeRequest.ApprovedByUserId = userId;
            ppeRequest.ApprovedAt = now;
            ppeRequest.RejectedByUserId = null;
            ppeRequest.RejectedAt = null;
            ppeRequest.RejectionReason = null;

            await _auditLogRepository.AddAsync(new AuditLog
            {
                EntityName = nameof(PPERequest),
                EntityId = ppeRequest.Id.ToString(),
                Action = AuditActions.PPERequestApproved,
                Description = $"PPE request {ppeRequest.Folio} approved.",
                OldValuesJson = oldValues,
                NewValuesJson = JsonSerializer.Serialize(new
                {
                    Status = ppeRequest.Status.ToString(),
                    ppeRequest.ApprovedByUserId,
                    ppeRequest.ApprovedAt,
                    ppeRequest.RejectedByUserId,
                    ppeRequest.RejectedAt,
                    ppeRequest.RejectionReason
                }),
                PerformedByUserId = userId,
                CreatedAt = now
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new ApprovePPERequestResultDto
            {
                Id = ppeRequest.Id,
                Folio = ppeRequest.Folio,
                Status = ppeRequest.Status,
                ApprovedByUserId = userId,
                ApprovedAt = now
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
