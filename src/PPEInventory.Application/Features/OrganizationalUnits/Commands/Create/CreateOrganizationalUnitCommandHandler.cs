using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.OrganizationalUnits.Commands.Create;

public class CreateOrganizationalUnitCommandHandler
    : IRequestHandler<
        CreateOrganizationalUnitCommand,
        OrganizationalUnitDto>
{
    private readonly IOrganizationalUnitRepository
        _repository;

    private readonly IDateTimeProvider
        _dateTimeProvider;

    public CreateOrganizationalUnitCommandHandler(
        IOrganizationalUnitRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<OrganizationalUnitDto> Handle(
        CreateOrganizationalUnitCommand request,
        CancellationToken cancellationToken)
    {
        var name =
            request.Name.Trim();

        OrganizationalUnit? parent = null;

        if (request.ParentId.HasValue)
        {
            parent =
                await _repository.GetByIdAsync(
                    request.ParentId.Value,
                    cancellationToken);

            if (parent is null)
            {
                throw new NotFoundException(
                    $"Organizational unit with id '{request.ParentId.Value}' was not found.");
            }

            if (!parent.IsActive)
            {
                throw new ConflictException(
                    $"Organizational unit '{parent.Name}' is inactive.");
            }
        }

        var alreadyExists =
            await _repository
                .ExistsByParentAndNameAsync(
                    request.ParentId,
                    name,
                    cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException(
                $"Organizational unit '{name}' already exists under the selected parent.");
        }

        var unit =
            new OrganizationalUnit
            {
                Name = name,

                Description =
                    Normalize(
                        request.Description),

                Type =
                    request.Type,

                ParentId =
                    parent?.Id,

                IsActive =
                    true,

                CreatedAt =
                    _dateTimeProvider.UtcNow
            };

        await _repository.AddAsync(
            unit,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new OrganizationalUnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Description = unit.Description,
            Type = unit.Type,

            ParentId =
                unit.ParentId,

            ParentName =
                parent?.Name,

            IsActive =
                unit.IsActive,

            CreatedAt =
                unit.CreatedAt,

            UpdatedAt =
                unit.UpdatedAt
        };
    }

    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}