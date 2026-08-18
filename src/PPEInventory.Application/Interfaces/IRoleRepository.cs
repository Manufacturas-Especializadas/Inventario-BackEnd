using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IRoleRepository
{
    Task<IReadOnlyList<Role>> GetByNamesAsync(
        IReadOnlyCollection<string> roleNames,
        CancellationToken cancellationToken = default);
}