using PPEInventory.Domain.Enums;

namespace PPEInventory.Domain.Entities;

public class OrganizationalUnit
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public OrganizationalUnitType Type{ get; set; }

    public int? ParentId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public OrganizationalUnit? Parent{ get; set; }

    public ICollection<OrganizationalUnit> Children
    {
        get;
        set;
    } = new List<OrganizationalUnit>();

    public ICollection<Employee> Employees
    {
        get;
        set;
    } = new List<Employee>();

    public ICollection<OrganizationalUnitPPELimit>
        PPELimits
    {
        get;
        set;
    } = new List<OrganizationalUnitPPELimit>();
}