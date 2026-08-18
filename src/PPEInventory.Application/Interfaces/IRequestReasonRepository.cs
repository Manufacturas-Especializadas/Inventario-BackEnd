using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IRequestReasonRepository
{
    Task<IReadOnlyList<RequestReason>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<RequestReason?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}