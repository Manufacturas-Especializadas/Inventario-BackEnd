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

    public ChangeWarehouseProductStatusCommandHandler(
        IWarehouseProductRepository repository,
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _inventoryRepository = inventoryRepository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WarehouseProductDto> Handle(
        ChangeWarehouseProductStatusCommand request,
        CancellationToken cancellationToken)
    {
        var relation =
            await _repository.GetAsync(
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
            return relation.ToDto();
        }


        // Si se va a reactivar, tanto almacén como producto
        // deben seguir activos.
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


        // Para desactivar, no puede quedar inventario.
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


        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");


        relation.IsActive = request.IsActive;
        relation.UpdatedAt = _dateTimeProvider.UtcNow;
        relation.UpdatedByUserId = userId;


        await _repository.SaveChangesAsync(
            cancellationToken);

        return relation.ToDto();
    }
}