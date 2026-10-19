using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.Employees;

public class EmployeeIdentityDto
{
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? OrganizationalUnitId { get; set; }
    public string? OrganizationalUnitName { get; set; }
    public OrganizationalUnitType? OrganizationalUnitType { get; set; }
}
