namespace PPEInventory.Application.Features.Users;

public class UserDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeNumber { get; set; }
        = string.Empty;

    public string EmployeeName { get; set; }
        = string.Empty;

    public string Username { get; set; }
        = string.Empty;

    public IReadOnlyCollection<string> Roles { get; set; }
        = Array.Empty<string>();

    public bool IsActive { get; set; }

    public bool EmployeeIsActive { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}