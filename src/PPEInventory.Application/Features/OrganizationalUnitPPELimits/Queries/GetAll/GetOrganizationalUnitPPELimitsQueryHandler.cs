using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.OrganizationalUnitPPELimits.Queries.GetAll;

public class GetOrganizationalUnitPPELimitsQueryHandler
    : IRequestHandler<
        GetOrganizationalUnitPPELimitsQuery,
        IReadOnlyList<
            OrganizationalUnitPPELimitDto>>
{
    private readonly
        IOrganizationalUnitPPELimitRepository
            _repository;

    public GetOrganizationalUnitPPELimitsQueryHandler(
        IOrganizationalUnitPPELimitRepository repository)
    {
        _repository = repository;
    }

    public async Task<
        IReadOnlyList<
            OrganizationalUnitPPELimitDto>>
        Handle(
            GetOrganizationalUnitPPELimitsQuery request,
            CancellationToken cancellationToken)
    {
        var limits =
            await _repository.GetAllAsync(
                request.OrganizationalUnitId,
                request.PPEProductId,
                cancellationToken);

        return limits
            .Select(x =>
                new OrganizationalUnitPPELimitDto
                {
                    Id = x.Id,

                    OrganizationalUnitId =
                        x.OrganizationalUnitId,

                    OrganizationalUnitName =
                        x.OrganizationalUnit.Name,

                    PPEProductId =
                        x.PPEProductId,

                    Sku =
                        x.PPEProduct.Sku,

                    ProductName =
                        x.PPEProduct.Name,

                    MaxQuantityPerCycle =
                        x.MaxQuantityPerCycle,

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