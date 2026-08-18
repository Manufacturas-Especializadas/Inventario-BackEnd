using MediatR;

namespace PPEInventory.Application.Features.Departments.Queries.GetAll;

public record GetDepartmentsQuery
    : IRequest<IReadOnlyList<DepartmentDto>>;