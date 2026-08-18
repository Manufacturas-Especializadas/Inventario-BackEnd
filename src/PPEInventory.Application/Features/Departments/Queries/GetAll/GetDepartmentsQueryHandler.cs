using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Departments.Queries.GetAll;

public class GetDepartmentsQueryHandler
    : IRequestHandler<
        GetDepartmentsQuery,
        IReadOnlyList<DepartmentDto>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentsQueryHandler(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IReadOnlyList<DepartmentDto>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var departments =
            await _departmentRepository.GetAllAsync(
                cancellationToken);

        return departments
            .Select(x => new DepartmentDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}