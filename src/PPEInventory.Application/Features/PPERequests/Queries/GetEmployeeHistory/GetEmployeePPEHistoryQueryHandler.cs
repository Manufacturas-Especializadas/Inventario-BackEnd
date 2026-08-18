using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetEmployeeHistory;

public class GetEmployeePPEHistoryQueryHandler
    : IRequestHandler<
        GetEmployeePPEHistoryQuery,
        IReadOnlyList<PPERequestDto>>
{
    private readonly IPPERequestRepository _requestRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeePPEHistoryQueryHandler(
        IPPERequestRepository requestRepository,
        IEmployeeRepository employeeRepository)
    {
        _requestRepository = requestRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<IReadOnlyList<PPERequestDto>> Handle(
        GetEmployeePPEHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var employeeNumber =
            request.EmployeeNumber.Trim();

        var employee =
            await _employeeRepository
                .GetByEmployeeNumberAsync(
                    employeeNumber,
                    cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                $"Employee '{employeeNumber}' was not found.");
        }

        var requests =
            await _requestRepository
                .GetHistoryByEmployeeNumberAsync(
                    employeeNumber,
                    cancellationToken);

        return requests
            .Select(x => x.ToDto())
            .ToArray();
    }
}

