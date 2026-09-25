using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetEmployeeHistory;

public class GetEmployeePPEHistoryQueryHandler
    : IRequestHandler<
        GetEmployeePPEHistoryQuery,
        PagedResult<PPERequestDto>>
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

    public async Task<PagedResult<PPERequestDto>> Handle(
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

        var result =
            await _requestRepository
                .GetHistoryPageByEmployeeNumberAsync(
                    employeeNumber,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

        return new PagedResult<PPERequestDto>
        {
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,

            Items =
                result.Items
                    .Select(x => x.ToDto())
                    .ToArray()
        };
    }
}

