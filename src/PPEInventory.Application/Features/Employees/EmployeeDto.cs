namespace PPEInventory.Application.Features.Employees;

public class EmployeeDto
{
    public int Id { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public int? LineId { get; set; }

    public string? LineName { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}