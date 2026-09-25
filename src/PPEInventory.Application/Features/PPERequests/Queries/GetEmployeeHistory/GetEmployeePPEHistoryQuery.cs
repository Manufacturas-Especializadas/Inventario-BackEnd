using MediatR;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetEmployeeHistory;

public record GetEmployeePPEHistoryQuery(
    string EmployeeNumber,
    int PageNumber = 1,
    int PageSize = PaginationParameters.DefaultPageSize)
    : IRequest<PagedResult<PPERequestDto>>;