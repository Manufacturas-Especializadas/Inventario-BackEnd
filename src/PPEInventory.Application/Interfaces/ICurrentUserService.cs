namespace PPEInventory.Application.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    int? UserId { get; }

    int? EmployeeId { get; }

    string? EmployeeNumber { get; }

    string? Name { get; }

    string? Username { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsInRole(string role);
}