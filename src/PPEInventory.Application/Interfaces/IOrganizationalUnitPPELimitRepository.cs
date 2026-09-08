using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IOrganizationalUnitPPELimitRepository
{
    Task<IReadOnlyList<
        OrganizationalUnitPPELimit>>
        GetAllAsync(
            int? organizationalUnitId,
            int? ppeProductId,
            CancellationToken cancellationToken = default);

    Task<OrganizationalUnitPPELimit?>
        GetByUnitAndProductAsync(
            int organizationalUnitId,
            int ppeProductId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<
        OrganizationalUnitPPELimit>>
        GetActiveByUnitsAndProductsAsync(
            IReadOnlyCollection<int>
                organizationalUnitIds,
            IReadOnlyCollection<int>
                ppeProductIds,
            CancellationToken cancellationToken = default);

    Task AddAsync(
        OrganizationalUnitPPELimit limit,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}