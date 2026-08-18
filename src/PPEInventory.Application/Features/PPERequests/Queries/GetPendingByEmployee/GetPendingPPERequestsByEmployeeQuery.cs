using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPendingByEmployee;

public record GetPendingPPERequestsByEmployeeQuery(
    string EmployeeNumber)
    : IRequest<IReadOnlyList<PPERequestDto>>;