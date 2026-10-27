namespace PPEInventory.Domain.Entities;

public class UserManagedOrganizationalUnit
{
    public int UserId { get; set; }
    public int OrganizationalUnitId { get; set; }
    public User User { get; set; } = null!;
    public OrganizationalUnit OrganizationalUnit { get; set; } = null!;
}
