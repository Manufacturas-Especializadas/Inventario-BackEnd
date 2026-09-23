using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.ProductColors.Commands.Update;

public class UpdateProductColorCommandHandler
    : IRequestHandler<
        UpdateProductColorCommand,
        ProductColorDto>
{
    private readonly IProductColorRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateProductColorCommandHandler(
        IProductColorRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProductColorDto> Handle(
        UpdateProductColorCommand request,
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

        var name = request.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name,
            color.Id,
            cancellationToken))
        {
            throw new ConflictException(
                $"A product color named '{name}' already exists.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        color.Name = name;

        color.UpdatedAt =
            _dateTimeProvider.UtcNow;

        color.UpdatedByUserId =
            userId;

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