namespace PPEInventory.Application.Features.Authentication;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public int UserId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; set; }
        = Array.Empty<string>();
}