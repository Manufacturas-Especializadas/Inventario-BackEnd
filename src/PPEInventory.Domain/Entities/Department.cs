namespace PPEInventory.Domain.Entities;

public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ProductionLine> ProductionLines { get; set; }
        = new List<ProductionLine>();

    public ICollection<Employee> Employees { get; set; }
        = new List<Employee>();
}