using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.PPEProducts.Commands.Create;

public class CreatePPEProductCommandHandler
    : IRequestHandler<
        CreatePPEProductCommand,
        PPEProductDto>
{
    private readonly IPPEProductRepository _productRepository;
    private readonly IPPECategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfMeasureRepository _unitRepository;

    public CreatePPEProductCommandHandler(
        IPPEProductRepository productRepository,
        IPPECategoryRepository categoryRepository,
        IUnitOfMeasureRepository unitRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitRepository = unitRepository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PPEProductDto> Handle(
        CreatePPEProductCommand request,
        CancellationToken cancellationToken)
    {
        var category =
            await _categoryRepository.GetByIdAsync(
                request.CategoryId,
                cancellationToken);

        if (category is null)
        {
            throw new NotFoundException(
                $"PPE category with id '{request.CategoryId}' was not found.");
        }

        if (!category.IsActive)
        {
            throw new ConflictException(
                $"PPE category '{category.Name}' is inactive.");
        }

        var stockUnit =
    await _unitRepository.GetByIdAsync(
        request.StockUnitId,
        cancellationToken);

        if (stockUnit is null)
        {
            throw new NotFoundException(
                $"Unit with id '{request.StockUnitId}' was not found.");
        }

        if (!stockUnit.IsActive)
        {
            throw new ConflictException(
                $"Unit '{stockUnit.Name}' is inactive.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var product = new PPEProduct
        {
            CategoryId = category.Id,

            Name = request.Name.Trim(),

            Description = Normalize(request.Description),

            Size = Normalize(request.Size),

            Color = Normalize(request.Color),

            Model = Normalize(request.Model),

            Specification =
                Normalize(request.Specification),

            StockUnitId = stockUnit.Id,

            MinimumStock =
                request.MinimumStock,

            DefaultMaxQuantityPerCycle =
                request.DefaultMaxQuantityPerCycle,

            ReplacementIntervalDays =
                request.ReplacementIntervalDays,

            IsActive = true,

            CreatedAt =
                _dateTimeProvider.UtcNow,

            CreatedByUserId =
                userId
        };

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        return new PPEProductDto
        {
            Id = product.Id,
            Sku = product.Sku,

            CategoryId = category.Id,
            CategoryName = category.Name,

            Name = product.Name,
            Description = product.Description,

            Size = product.Size,
            Color = product.Color,
            Model = product.Model,
            Specification = product.Specification,

            StockUnitId = stockUnit.Id,
            StockUnit = stockUnit.Name,
            StockUnitSymbol = stockUnit.Symbol,

            MinimumStock = product.MinimumStock,

            DefaultMaxQuantityPerCycle =
                product.DefaultMaxQuantityPerCycle,

            ReplacementIntervalDays =
                product.ReplacementIntervalDays,

            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}