using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.PPEProducts.Commands.Update;

public class UpdatePPEProductCommandHandler
    : IRequestHandler<
        UpdatePPEProductCommand,
        PPEProductDto>
{
    private readonly IPPEProductRepository
        _productRepository;

    private readonly IPPECategoryRepository
        _categoryRepository;

    private readonly IUnitOfMeasureRepository
        _unitRepository;

    private readonly IProductSizeRepository
        _sizeRepository;

    private readonly IProductColorRepository
        _colorRepository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public UpdatePPEProductCommandHandler(
        IPPEProductRepository productRepository,
        IPPECategoryRepository categoryRepository,
        IUnitOfMeasureRepository unitRepository,
        IProductSizeRepository sizeRepository,
        IProductColorRepository colorRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _productRepository =
            productRepository;

        _categoryRepository =
            categoryRepository;

        _unitRepository =
            unitRepository;

        _sizeRepository =
            sizeRepository;

        _colorRepository =
            colorRepository;

        _currentUser =
            currentUser;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<PPEProductDto> Handle(
        UpdatePPEProductCommand request,
        CancellationToken cancellationToken)
    {
        var product =
            await _productRepository
                .GetByIdForUpdateAsync(
                    request.Id,
                    cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"PPE product with id '{request.Id}' was not found.");
        }


        var category =
            await _categoryRepository
                .GetByIdAsync(
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
            await _unitRepository
                .GetByIdAsync(
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


        ProductSize? productSize = null;

        if (request.SizeId.HasValue)
        {
            productSize =
                await _sizeRepository
                    .GetByIdAsync(
                        request.SizeId.Value,
                        cancellationToken);

            if (productSize is null)
            {
                throw new NotFoundException(
                    $"Product size with id '{request.SizeId.Value}' was not found.");
            }

            if (!productSize.IsActive)
            {
                throw new ConflictException(
                    $"Product size '{productSize.Name}' is inactive.");
            }
        }


        ProductColor? productColor = null;

        if (request.ColorId.HasValue)
        {
            productColor =
                await _colorRepository
                    .GetByIdAsync(
                        request.ColorId.Value,
                        cancellationToken);

            if (productColor is null)
            {
                throw new NotFoundException(
                    $"Product color with id '{request.ColorId.Value}' was not found.");
            }

            if (!productColor.IsActive)
            {
                throw new ConflictException(
                    $"Product color '{productColor.Name}' is inactive.");
            }
        }


        var name =
            request.Name.Trim();

        var description =
            Normalize(
                request.Description);

        var model =
            Normalize(
                request.Model);

        var specification =
            Normalize(
                request.Specification);


        var nothingChanged =
            product.CategoryId ==
                category.Id &&

            string.Equals(
                product.Name,
                name,
                StringComparison.Ordinal) &&

            string.Equals(
                product.Description,
                description,
                StringComparison.Ordinal) &&

            product.SizeId ==
                productSize?.Id &&

            product.ColorId ==
                productColor?.Id &&

            string.Equals(
                product.Model,
                model,
                StringComparison.Ordinal) &&

            string.Equals(
                product.Specification,
                specification,
                StringComparison.Ordinal) &&

            product.StockUnitId ==
                stockUnit.Id &&

            product.MinimumStock ==
                request.MinimumStock &&

            product.DefaultMaxQuantityPerCycle ==
                request.DefaultMaxQuantityPerCycle &&

            product.ReplacementIntervalDays ==
                request.ReplacementIntervalDays;


        if (nothingChanged)
        {
            return Map(
                product,
                category,
                productSize,
                productColor,
                stockUnit);
        }


        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");


        product.CategoryId =
            category.Id;

        product.Name =
            name;

        product.Description =
            description;

        product.SizeId =
            productSize?.Id;

        product.ColorId =
            productColor?.Id;

        product.Model =
            model;

        product.Specification =
            specification;

        product.StockUnitId =
            stockUnit.Id;

        product.MinimumStock =
            request.MinimumStock;

        product.DefaultMaxQuantityPerCycle =
            request.DefaultMaxQuantityPerCycle;

        product.ReplacementIntervalDays =
            request.ReplacementIntervalDays;

        product.UpdatedAt =
            _dateTimeProvider.UtcNow;

        product.UpdatedByUserId =
            userId;


        await _productRepository
            .SaveChangesAsync(
                cancellationToken);


        return Map(
            product,
            category,
            productSize,
            productColor,
            stockUnit);
    }


    private static PPEProductDto Map(
        PPEProduct product,
        PPECategory category,
        ProductSize? size,
        ProductColor? color,
        UnitOfMeasure stockUnit)
    {
        return new PPEProductDto
        {
            Id = product.Id,
            Sku = product.Sku,

            CategoryId =
                category.Id,

            CategoryName =
                category.Name,

            Name =
                product.Name,

            Description =
                product.Description,

            SizeId =
                size?.Id,

            Size =
                size?.Name,

            ColorId =
                color?.Id,

            Color =
                color?.Name,

            Model =
                product.Model,

            Specification =
                product.Specification,

            StockUnitId =
                stockUnit.Id,

            StockUnit =
                stockUnit.Name,

            StockUnitSymbol =
                stockUnit.Symbol,

            MinimumStock =
                product.MinimumStock,

            DefaultMaxQuantityPerCycle =
                product.DefaultMaxQuantityPerCycle,

            ReplacementIntervalDays =
                product.ReplacementIntervalDays,

            IsActive =
                product.IsActive,

            CreatedAt =
                product.CreatedAt
        };
    }


    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
            value)
            ? null
            : value.Trim();
    }
}