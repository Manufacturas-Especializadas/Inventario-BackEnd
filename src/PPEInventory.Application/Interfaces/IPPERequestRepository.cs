using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Features.PPERequests;
using PPEInventory.Application.Features.PPERequests.Queries.GetDeliveryHistory;
using PPEInventory.Application.Features.PPERequests.Queries.GetPending;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Interfaces;

public interface IPPERequestRepository
{
    Task<int> CountByStatusAsync(
        PPERequestStatus status,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PPERequest request,
        CancellationToken cancellationToken = default);

    Task<PPERequest?> GetByFolioAsync(
        string folio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Acquires an update lock in the caller's transaction. Call before loading
    /// the request into the change tracker so its state reflects the locked read.
    /// </summary>
    Task<PPERequest?> GetByFolioForUpdateAsync(
    string folio,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PPERequest>>
    GetPendingByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PendingPPERequestDto>> GetPendingAsync(
        GetPendingPPERequestsQuery filters,
        CancellationToken cancellationToken = default);

    Task<PagedResult<DeliveryHistoryPPERequestDto>>
    GetDeliveryHistoryAsync(
        GetDeliveryHistoryQuery filters,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PendingApprovalPPERequestDto>> GetPendingApprovalPageAsync(
        int pageNumber,
        int pageSize,
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
