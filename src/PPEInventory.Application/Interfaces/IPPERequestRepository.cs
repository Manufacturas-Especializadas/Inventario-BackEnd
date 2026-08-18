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

    Task<DateTime?> GetLastDeliveredAtAsync(
        int employeeId,
        int ppeProductId,
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
}