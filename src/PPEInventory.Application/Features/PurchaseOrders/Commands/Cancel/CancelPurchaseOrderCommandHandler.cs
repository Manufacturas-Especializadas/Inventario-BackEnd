using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Cancel;

public class CancelPurchaseOrderCommandHandler
    : IRequestHandler<
        CancelPurchaseOrderCommand,
        PurchaseOrderDto>
{
    private readonly
        IPurchaseOrderRepository
        _repository;

    private readonly
        ICurrentUserService
        _currentUser;

    private readonly
        IDateTimeProvider
        _dateTimeProvider;

    private readonly IUnitOfWork _unitOfWork;

    public CancelPurchaseOrderCommandHandler(
        IPurchaseOrderRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderDto> Handle(
    CancelPurchaseOrderCommand request,
    CancellationToken cancellationToken)
    {
        var folio =
            request.Folio
                .Trim()
                .ToUpperInvariant();

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
            var po =
                await _repository
                    .GetByFolioForUpdateAsync(
                        folio,
                        cancellationToken);

            if (po is null)
            {
                throw new NotFoundException(
                    $"Purchase order '{folio}' was not found.");
            }

            if (po.Status ==
                PurchaseOrderStatus.Received)
            {
                throw new ConflictException(
                    "A received purchase order cannot be cancelled.");
            }

            if (po.Status ==
                PurchaseOrderStatus.Cancelled)
            {
                throw new ConflictException(
                    "Purchase order is already cancelled.");
            }

            var now =
                _dateTimeProvider.UtcNow;

            po.Status =
                PurchaseOrderStatus.Cancelled;

            po.CancelledAt =
                now;

            po.CancelledByUserId =
                userId;

            po.CancellationReason =
                request.Reason.Trim();

            po.UpdatedAt =
                now;

            po.UpdatedByUserId =
                userId;

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken);

            await transaction
                .CommitAsync(
                    cancellationToken);

            return new PurchaseOrderDto
            {
                Id = po.Id,
                Folio = po.Folio,

                SupplierId = po.SupplierId,
                SupplierName = po.Supplier.Name,

                PurchaseOrderNumber =
                    po.PurchaseOrderNumber,

                Status = po.Status,

                OrderDate = po.OrderDate,

                ConfirmedDeliveryDate =
                    po.ConfirmedDeliveryDate,

                SupplierConfirmedAt =
                    po.SupplierConfirmedAt,

                CurrencyCode =
                    po.CurrencyCode,

                Notes = po.Notes,

                CreatedAt = po.CreatedAt,
                UpdatedAt = po.UpdatedAt,

                CancelledAt = po.CancelledAt,

                CancellationReason =
                    po.CancellationReason,

                Items = po.Items
                    .Select(item =>
                        new PurchaseOrderItemDto
                        {
                            Id = item.Id,

                            PPEProductId =
                                item.PPEProductId,

                            Sku =
                                item.PPEProduct.Sku,

                            ProductName =
                                item.PPEProduct.Name,

                            SupplierProductCode =
                                item.SupplierProductCode,

                            PurchaseUnit =
                                item.PurchaseUnit,

                            UnitsPerPackage =
                                item.UnitsPerPackage,

                            OrderedPurchaseQuantity =
                                item.OrderedPurchaseQuantity,

                            PurchaseUnitCost =
                                item.PurchaseUnitCost
                        })
                    .ToArray()
            };
        }
        catch
        {
            await transaction
                .RollbackAsync(
                    cancellationToken);

            throw;
        }
    }
}