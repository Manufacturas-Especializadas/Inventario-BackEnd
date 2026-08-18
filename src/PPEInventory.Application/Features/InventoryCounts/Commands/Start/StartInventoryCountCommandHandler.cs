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
    private readonly IPPEProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StartInventoryCountCommandHandler(
        IInventoryCountRepository countRepository,
        IWarehouseRepository warehouseRepository,
        IPPEProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _countRepository = countRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
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

        var products =
            await _productRepository.GetAllAsync(
                cancellationToken);

        var activeProducts =
            products
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToArray();

        if (activeProducts.Length == 0)
        {
            throw new ConflictException(
                "There are no active PPE products to count.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var count =
            new InventoryCount
            {
                WarehouseId =
                    warehouse.Id,

                Status =
                    InventoryCountStatus.Draft,

                Notes =
                    Normalize(command.Notes),

                CreatedByUserId =
                    userId,

                CreatedAt =
                    _dateTimeProvider.UtcNow
            };

        foreach (var product in activeProducts)
        {
            count.Items.Add(
                new InventoryCountItem
                {
                    PPEProductId =
                        product.Id
                });
        }

        await _countRepository.AddAsync(
            count,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        // Las navegaciones utilizadas por el mapping
        // las asignamos porque acabamos de crear el objeto.
        count.Warehouse = warehouse;

        foreach (var item in count.Items)
        {
            item.PPEProduct =
                activeProducts.First(
                    x => x.Id == item.PPEProductId);
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