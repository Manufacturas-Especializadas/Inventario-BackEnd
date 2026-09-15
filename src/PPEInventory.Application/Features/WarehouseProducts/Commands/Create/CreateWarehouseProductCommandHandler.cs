using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.WarehouseProducts.Commands.Create;

public class CreateWarehouseProductCommandHandler
    : IRequestHandler<
        CreateWarehouseProductCommand,
        WarehouseProductDto>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IPPEProductRepository _productRepository;
    private readonly IWarehouseProductRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateWarehouseProductCommandHandler(
        IWarehouseRepository warehouseRepository,
        IPPEProductRepository productRepository,
        IWarehouseProductRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WarehouseProductDto> Handle(
        CreateWarehouseProductCommand request,
        CancellationToken cancellationToken)
    {
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


        var product =
            await _productRepository.GetByIdAsync(
                request.PPEProductId,
                cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"PPE product with id '{request.PPEProductId}' was not found.");
        }

        if (!product.IsActive)
        {
            throw new ConflictException(
                $"PPE product '{product.Sku}' is inactive.");
        }


        if (await _repository.ExistsAsync(
            warehouse.Id,
            product.Id,
            cancellationToken))
        {
            throw new ConflictException(
                $"Product '{product.Sku}' is already assigned to warehouse '{warehouse.Name}'.");
        }


        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");


        var relation = new WarehouseProduct
        {
            WarehouseId = warehouse.Id,
            PPEProductId = product.Id,

            IsActive = true,

            CreatedAt = _dateTimeProvider.UtcNow,
            CreatedByUserId = userId
        };


        await _repository.AddAsync(
            relation,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);


        relation.Warehouse = warehouse;
        relation.PPEProduct = product;

        return relation.ToDto();
    }
}