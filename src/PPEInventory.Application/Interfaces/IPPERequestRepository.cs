using PPEInventory.Application.Common.Models;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Interfaces;

public interface IPPERequestRepository
{
    Task AddAsync(
        PPERequest request,
        CancellationToken cancellationToken = default);

    Task<PPERequest?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default);

    Task<PPERequest?> GetByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PPERequest>>
    GetPendingByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PPERequest>> GetPendingAsync(
    int? warehouseId,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PPERequest>>
    GetHistoryByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task<int> GetCommittedNormalQuantityInCycleAsync(
    int requestedForOrganizationalUnitId,
    int ppeProductId,
    DateTime cycleStart,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<int, int>>
    GetCommittedNormalQuantitiesInCycleAsync(
        int requestedForOrganizationalUnitId,
        IReadOnlyCollection<int> ppeProductIds,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PPERequest>>
    GetHistoryPageByEmployeeNumberAsync(
        string employeeNumber,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

}