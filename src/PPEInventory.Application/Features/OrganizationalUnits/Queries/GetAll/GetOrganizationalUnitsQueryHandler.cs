using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.OrganizationalUnits.Queries.GetAll;

public class GetOrganizationalUnitsQueryHandler
    : IRequestHandler<
        GetOrganizationalUnitsQuery,
        IReadOnlyList<OrganizationalUnitDto>>
{
    private readonly IOrganizationalUnitRepository
        _repository;

    public GetOrganizationalUnitsQueryHandler(
        IOrganizationalUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<
        IReadOnlyList<OrganizationalUnitDto>>
        Handle(
            GetOrganizationalUnitsQuery request,
            CancellationToken cancellationToken)
    {
        var units =
            await _repository.GetAllAsync(
                cancellationToken);

        return units
            .Select(x =>
                new OrganizationalUnitDto
                {
                    Id = x.Id,
                    Name = x.Name,

                    Description =
                        x.Description,

                    Type = x.Type,

                    ParentId =
                        x.ParentId,

                    ParentName =
                        x.Parent?.Name,

                    IsActive =
                        x.IsActive,

                    CreatedAt =
                        x.CreatedAt,

                    UpdatedAt =
                        x.UpdatedAt
                })
            .ToArray();
    }
}