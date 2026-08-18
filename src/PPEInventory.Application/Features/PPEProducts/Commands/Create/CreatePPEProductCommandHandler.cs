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

    public CreatePPEProductCommandHandler(
        IPPEProductRepository productRepository,
        IPPECategoryRepository categoryRepository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
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

            StockUnit = request.StockUnit.Trim(),

            MinimumStock =
                request.MinimumStock,

            MaxQuantityPerRequest =
                request.MaxQuantityPerRequest,

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

            StockUnit = product.StockUnit,

            MinimumStock = product.MinimumStock,

            MaxQuantityPerRequest =
                product.MaxQuantityPerRequest,

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