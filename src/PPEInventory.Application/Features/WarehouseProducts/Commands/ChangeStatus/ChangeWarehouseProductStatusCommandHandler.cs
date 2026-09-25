using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.WarehouseProducts.Commands.ChangeStatus;

public class ChangeWarehouseProductStatusCommandHandler
    : IRequestHandler<
        ChangeWarehouseProductStatusCommand,
        WarehouseProductDto>
{
    private readonly IWarehouseProductRepository _repository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeWarehouseProductStatusCommandHandler(
        IWarehouseProductRepository repository,
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _inventoryRepository = inventoryRepository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseProductDto> Handle(
    ChangeWarehouseProductStatusCommand request,
    CancellationToken cancellationToken)
    {
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
            var relation =
                await _repository.GetForUpdateAsync(
                    request.WarehouseId,
                    request.PPEProductId,
                    cancellationToken);

            if (relation is null)
            {
                throw new NotFoundException(
                    "The product is not assigned to the warehouse.");
            }

            // No hay nada que modificar.
            if (relation.IsActive == request.IsActive)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return relation.ToDto();
            }

            // Para reactivar, almacén y producto deben seguir activos.
            if (request.IsActive)
            {
                if (!relation.Warehouse.IsActive)
                {
                    throw new ConflictException(
                        $"Warehouse '{relation.Warehouse.Name}' is inactive.");
                }

                if (!relation.PPEProduct.IsActive)
                {
                    throw new ConflictException(
                        $"PPE product '{relation.PPEProduct.Sku}' is inactive.");
                }
            }

            // Para desactivar no puede existir inventario
            // ni cantidad reservada.
            if (!request.IsActive)
            {
                var balance =
                    await _inventoryRepository.GetBalanceAsync(
                        relation.WarehouseId,
                        relation.PPEProductId,
                        cancellationToken);

                if (balance is not null &&
                    (balance.OnHandQuantity > 0 ||
                     balance.ReservedQuantity > 0))
                {
                    throw new ConflictException(
                        $"Product '{relation.PPEProduct.Sku}' cannot be removed from warehouse " +
                        $"'{relation.Warehouse.Name}' because it still has inventory or reserved quantity.");
                }
            }

            relation.IsActive = request.IsActive;
            relation.UpdatedAt =
                _dateTimeProvider.UtcNow;
            relation.UpdatedByUserId =
                userId;

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return relation.ToDto();
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}