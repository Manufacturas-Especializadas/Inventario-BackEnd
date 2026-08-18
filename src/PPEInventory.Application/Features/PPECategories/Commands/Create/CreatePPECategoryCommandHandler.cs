using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.PPECategories.Commands.Create;

public class CreatePPECategoryCommandHandler
    : IRequestHandler<
        CreatePPECategoryCommand,
        PPECategoryDto>
{
    private readonly IPPECategoryRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreatePPECategoryCommandHandler(
        IPPECategoryRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PPECategoryDto> Handle(
        CreatePPECategoryCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name,
            cancellationToken))
        {
            throw new ConflictException(
                $"PPE category '{name}' already exists.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var category = new PPECategory
        {
            Name = name,

            Description =
                string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),

            IsActive = true,

            CreatedAt =
                _dateTimeProvider.UtcNow,

            CreatedByUserId =
                userId
        };

        await _repository.AddAsync(
            category,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new PPECategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}