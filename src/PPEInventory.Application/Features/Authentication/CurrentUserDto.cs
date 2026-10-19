using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.Authentication;

public class CurrentUserDto
{
    public bool IsAuthenticated { get; set; }
    public int UserId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Username { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
    public int? OrganizationalUnitId { get; set; }
    public string? OrganizationalUnitName { get; set; }
    public OrganizationalUnitType? OrganizationalUnitType { get; set; }
}
