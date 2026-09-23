using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.ProductColors.Commands.SetStatus;

public class SetProductColorStatusCommandHandler
    : IRequestHandler<
        SetProductColorStatusCommand,
        ProductColorDto>
{
    private readonly IProductColorRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SetProductColorStatusCommandHandler(
        IProductColorRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProductColorDto> Handle(
        SetProductColorStatusCommand request,
        CancellationToken cancellationToken)
    {
        var color =
            await _repository.GetByIdForUpdateAsync(
                request.Id,
                cancellationToken);

        if (color is null)
        {
            throw new NotFoundException(
                $"Product color with id '{request.Id}' was not found.");
        }

        if (color.IsActive == request.IsActive)
        {
            return Map(color);
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        color.IsActive =
            request.IsActive;

        color.UpdatedAt =
            _dateTimeProvider.UtcNow;

        color.UpdatedByUserId =
            userId;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Map(color);
    }

    private static ProductColorDto Map(
        ProductColor color)
    {
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