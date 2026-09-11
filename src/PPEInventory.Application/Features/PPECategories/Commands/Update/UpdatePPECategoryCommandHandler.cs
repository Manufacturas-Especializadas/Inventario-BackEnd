using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPECategories.Commands.Update;

public class UpdatePPECategoryCommandHandler
    : IRequestHandler<
        UpdatePPECategoryCommand,
        PPECategoryDto>
{
    private readonly IPPECategoryRepository
        _repository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public UpdatePPECategoryCommandHandler(
        IPPECategoryRepository repository,
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


    public async Task<PPECategoryDto> Handle(
        UpdatePPECategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category =
            await _repository
                .GetByIdForUpdateAsync(
                    request.Id,
                    cancellationToken);


        if (category is null)
        {
            throw new NotFoundException(
                $"PPE category with id '{request.Id}' was not found.");
        }


        var name =
            request.Name.Trim();

        var description =
            string.IsNullOrWhiteSpace(
                request.Description)
                ? null
                : request.Description.Trim();


        var duplicateExists =
            await _repository
                .ExistsByNameAsync(
                    name,
                    category.Id,
                    cancellationToken);


        if (duplicateExists)
        {
            throw new ConflictException(
                $"PPE category '{name}' already exists.");
        }


        /*
         * Si realmente no cambió nada,
         * devolvemos la categoría sin modificar
         * UpdatedAt ni UpdatedByUserId.
         */
        if (
            string.Equals(
                category.Name,
                name,
                StringComparison.Ordinal) &&
            string.Equals(
                category.Description,
                description,
                StringComparison.Ordinal))
        {
            return new PPECategoryDto
            {
                Id =
                    category.Id,

                Name =
                    category.Name,

                Description =
                    category.Description,

                IsActive =
                    category.IsActive,

                CreatedAt =
                    category.CreatedAt
            };
        }


        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");


        category.Name =
            name;

        category.Description =
            description;

        category.UpdatedAt =
            _dateTimeProvider.UtcNow;

        category.UpdatedByUserId =
            userId;


        await _repository
            .SaveChangesAsync(
                cancellationToken);


        return new PPECategoryDto
        {
            Id =
                category.Id,

            Name =
                category.Name,

            Description =
                category.Description,

            IsActive =
                category.IsActive,

            CreatedAt =
                category.CreatedAt
        };
    }
}