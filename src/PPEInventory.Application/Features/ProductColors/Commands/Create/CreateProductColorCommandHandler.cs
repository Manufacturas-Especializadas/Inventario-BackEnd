using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.ProductColors.Commands.Create;

public class CreateProductColorCommandHandler
    : IRequestHandler<
        CreateProductColorCommand,
        ProductColorDto>
{
    private readonly IProductColorRepository
        _repository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;

    public CreateProductColorCommandHandler(
        IProductColorRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProductColorDto> Handle(
        CreateProductColorCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (
            await _repository.ExistsByNameAsync(
                name,
                cancellationToken)
        )
        {
            throw new ConflictException(
                $"Product color '{name}' already exists.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var color =
            new ProductColor
            {
                Name = name,
                IsActive = true,
                CreatedAt =
                    _dateTimeProvider.UtcNow,
                CreatedByUserId =
                    userId
            };

        await _repository.AddAsync(
            color,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new ProductColorDto
        {
            Id = color.Id,
            Name = color.Name,
            IsActive = color.IsActive,
            CreatedAt = color.CreatedAt,
            UpdatedAt = color.UpdatedAt
        };
    }
}