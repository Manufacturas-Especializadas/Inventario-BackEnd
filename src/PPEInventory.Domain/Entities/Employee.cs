namespace PPEInventory.Domain.Entities;

public class Employee
{
    public int Id { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public int? LineId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Department Department { get; set; } = null!;

    public ProductionLine? Line { get; set; }

    public User? User { get; set; }
}