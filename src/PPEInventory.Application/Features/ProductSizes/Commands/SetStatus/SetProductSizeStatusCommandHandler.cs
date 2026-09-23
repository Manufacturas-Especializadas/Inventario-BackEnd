using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.ProductSizes.Commands.SetStatus;

public class SetProductSizeStatusCommandHandler
    : IRequestHandler<
        SetProductSizeStatusCommand,
        ProductSizeDto>
{
    private readonly IProductSizeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SetProductSizeStatusCommandHandler(
        IProductSizeRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProductSizeDto> Handle(
        SetProductSizeStatusCommand request,
        CancellationToken cancellationToken)
    {
        var size =
            await _repository.GetByIdForUpdateAsync(
                request.Id,
                cancellationToken);

        if (size is null)
        {
            throw new NotFoundException(
                $"Product size with id '{request.Id}' was not found.");
        }

        if (size.IsActive == request.IsActive)
        {
            return Map(size);
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        size.IsActive =
            request.IsActive;

        size.UpdatedAt =
            _dateTimeProvider.UtcNow;

        size.UpdatedByUserId =
            userId;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Map(size);
    }

    private static ProductSizeDto Map(
        PPEInventory.Domain.Entities.ProductSize size)
    {
        return new ProductSizeDto
        {
            Id = size.Id,
            Name = size.Name,
            IsActive = size.IsActive,
            CreatedAt = size.CreatedAt,
            UpdatedAt = size.UpdatedAt
        };
    }
}