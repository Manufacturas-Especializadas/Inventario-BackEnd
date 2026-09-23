using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.ProductSizes.Commands.Create;

public class CreateProductSizeCommandHandler
    : IRequestHandler<
        CreateProductSizeCommand,
        ProductSizeDto>
{
    private readonly IProductSizeRepository
        _repository;

    private readonly ICurrentUserService
    _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;

    public CreateProductSizeCommandHandler(
        IProductSizeRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider =
            dateTimeProvider;
    }

    public async Task<ProductSizeDto> Handle(
        CreateProductSizeCommand request,
        CancellationToken cancellationToken)
    {
        var name =
            request.Name.Trim();

        if (
            await _repository
                .ExistsByNameAsync(
                    name,
                    cancellationToken)
        )
        {
            throw new ConflictException(
                $"Product size '{name}' already exists.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var size =
            new ProductSize
            {
                Name = name,
                IsActive = true,
                CreatedAt =
                    _dateTimeProvider.UtcNow,
                CreatedByUserId =
                    userId
            };

        await _repository.AddAsync(
            size,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new ProductSizeDto
        {
            Id = size.Id,
            Name = size.Name,
            IsActive = size.IsActive,
            CreatedAt =
                size.CreatedAt,
            UpdatedAt =
                size.UpdatedAt
        };
    }
}