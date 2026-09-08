using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPendingByEmployee;

public class GetPendingPPERequestsByEmployeeQueryHandler
    : IRequestHandler<
        GetPendingPPERequestsByEmployeeQuery,
        IReadOnlyList<PPERequestDto>>
{
    private readonly IPPERequestRepository _repository;

    public GetPendingPPERequestsByEmployeeQueryHandler(
        IPPERequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PPERequestDto>> Handle(
        GetPendingPPERequestsByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var employeeNumber =
            request.EmployeeNumber.Trim();

        var requests =
            await _repository
                .GetPendingByEmployeeNumberAsync(
                    employeeNumber,
                    cancellationToken);

        return requests
    .Select(x => x.ToDto())
    .ToArray();
    }
}