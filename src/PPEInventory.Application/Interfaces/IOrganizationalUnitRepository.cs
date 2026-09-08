using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IOrganizationalUnitRepository
{
    Task<IReadOnlyList<OrganizationalUnit>>
        GetAllAsync(
            CancellationToken cancellationToken = default);

    Task<OrganizationalUnit?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByParentAndNameAsync(
        int? parentId,
        string name,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrganizationalUnit>>
        GetPathToRootAsync(
            int organizationalUnitId,
            CancellationToken cancellationToken = default);

    Task AddAsync(
        OrganizationalUnit organizationalUnit,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}