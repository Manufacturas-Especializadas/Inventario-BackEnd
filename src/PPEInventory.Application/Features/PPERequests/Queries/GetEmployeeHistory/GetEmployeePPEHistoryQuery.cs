using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetEmployeeHistory;

public record GetEmployeePPEHistoryQuery(
    string EmployeeNumber)
    : IRequest<IReadOnlyList<PPERequestDto>>;