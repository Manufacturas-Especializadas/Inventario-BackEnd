using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPECategories.Commands.SetStatus;

public class SetPPECategoryStatusCommandHandler
    : IRequestHandler<
        SetPPECategoryStatusCommand,
        PPECategoryDto>
{
    private readonly IPPECategoryRepository
        _repository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public SetPPECategoryStatusCommandHandler(
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
        SetPPECategoryStatusCommand request,
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


        /*
         * Si ya tiene el estado solicitado,
         * no generamos una actualización falsa.
         */
        if (
            category.IsActive ==
            request.IsActive)
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


        category.IsActive =
            request.IsActive;

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