using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.PPEProducts.Commands.SetStatus;

public class SetPPEProductStatusCommandHandler
    : IRequestHandler<
        SetPPEProductStatusCommand,
        PPEProductDto>
{
    private readonly IPPEProductRepository
        _repository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public SetPPEProductStatusCommandHandler(
        IPPEProductRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository =
            repository;

        _currentUser =
            currentUser;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<PPEProductDto> Handle(
        SetPPEProductStatusCommand request,
        CancellationToken cancellationToken)
    {
        var product =
            await _repository
                .GetByIdForUpdateAsync(
                    request.Id,
                    cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"PPE product with id '{request.Id}' was not found.");
        }


        if (
            product.IsActive !=
            request.IsActive)
        {
            var userId =
                _currentUser.UserId
                ?? throw new UnauthorizedException(
                    "Authenticated user was not found.");

            product.IsActive =
                request.IsActive;

            product.UpdatedAt =
                _dateTimeProvider.UtcNow;

            product.UpdatedByUserId =
                userId;

            await _repository
                .SaveChangesAsync(
                    cancellationToken);
        }


        return Map(product);
    }


    private static PPEProductDto Map(
        PPEProduct product)
    {
        return new PPEProductDto
        {
            Id =
                product.Id,

            Sku =
                product.Sku,

            CategoryId =
                product.CategoryId,

            CategoryName =
                product.Category.Name,

            Name =
                product.Name,

            Description =
                product.Description,

            SizeId =
                product.SizeId,

            Size =
                product.ProductSize?.Name,

            ColorId =
                product.ColorId,

            Color =
                product.ProductColor?.Name,

            Model =
                product.Model,

            Specification =
                product.Specification,

            StockUnitId =
                product.StockUnitId,

            StockUnit =
                product.StockUnitOfMeasure.Name,

            StockUnitSymbol =
                product.StockUnitOfMeasure.Symbol,

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
}