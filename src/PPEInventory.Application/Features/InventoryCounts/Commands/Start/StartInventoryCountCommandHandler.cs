using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.Start;

public class StartInventoryCountCommandHandler
    : IRequestHandler<
        StartInventoryCountCommand,
        InventoryCountDto>
{
    private readonly IInventoryCountRepository _countRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWarehouseProductRepository _warehouseProductRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;


    public StartInventoryCountCommandHandler(
        IInventoryCountRepository countRepository,
        IWarehouseRepository warehouseRepository,
        IWarehouseProductRepository warehouseProductRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _countRepository = countRepository;
        _warehouseRepository = warehouseRepository;
        _warehouseProductRepository = warehouseProductRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }


    public async Task<InventoryCountDto> Handle(
        StartInventoryCountCommand command,
        CancellationToken cancellationToken)
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


        if (await _countRepository.HasOpenCountAsync(
            warehouse.Id,
            cancellationToken))
        {
            throw new ConflictException(
                $"Warehouse '{warehouse.Name}' already has an open inventory count.");
        }


        var warehouseProducts =
            await _warehouseProductRepository
                .GetActiveByWarehouseIdAsync(
                    warehouse.Id,
                    cancellationToken);


        if (warehouseProducts.Count == 0)
        {
            throw new ConflictException(
                $"Warehouse '{warehouse.Name}' has no active products configured for inventory counting.");
        }


        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");


        var count =
            new InventoryCount
            {
                WarehouseId = warehouse.Id,

                Status = InventoryCountStatus.Draft,

                Notes = Normalize(command.Notes),

                CreatedByUserId = userId,

                CreatedAt = _dateTimeProvider.UtcNow
            };


        foreach (var warehouseProduct in warehouseProducts)
        {
            count.Items.Add(
                new InventoryCountItem
                {
                    PPEProductId =
                        warehouseProduct.PPEProductId
                });
        }


        await _countRepository.AddAsync(
            count,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);


        // Navegaciones necesarias para mapear inmediatamente
        // el conteo recién creado al DTO.
        count.Warehouse = warehouse;

        foreach (var item in count.Items)
        {
            item.PPEProduct =
                warehouseProducts
                    .First(x =>
                        x.PPEProductId ==
                        item.PPEProductId)
                    .PPEProduct;
        }


        return count.ToDto();
    }


    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}