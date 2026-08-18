namespace PPEInventory.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Employee Employee { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
}