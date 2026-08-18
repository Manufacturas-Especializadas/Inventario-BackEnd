namespace PPEInventory.Domain.Entities;

public class ProductionLine
{
    public int Id { get; set; }

    public int DepartmentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Department Department { get; set; } = null!;

    public ICollection<Employee> Employees { get; set; }
        = new List<Employee>();
}