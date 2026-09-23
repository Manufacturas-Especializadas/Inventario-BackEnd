using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.ProductSizes.Commands.Update;

public class UpdateProductSizeCommandHandler
    : IRequestHandler<
        UpdateProductSizeCommand,
        ProductSizeDto>
{
    private readonly IProductSizeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateProductSizeCommandHandler(
        IProductSizeRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProductSizeDto> Handle(
        UpdateProductSizeCommand request,
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

        var name = request.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name,
            size.Id,
            cancellationToken))
        {
            throw new ConflictException(
                $"A product size named '{name}' already exists.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        size.Name = name;

        size.UpdatedAt =
            _dateTimeProvider.UtcNow;

        size.UpdatedByUserId =
            userId;

        await _repository.SaveChangesAsync(
            cancellationToken);

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