using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Users;

public static class UserMappings
{
    public static UserDto ToDto(
        this User user)
    {
        return new UserDto
        {
            Id =
                user.Id,

            EmployeeId =
                user.EmployeeId,

            EmployeeNumber =
                user.Employee.EmployeeNumber,

            EmployeeName =
                user.Employee.Name,

            Username =
                user.Username,

            Roles =
                user.UserRoles
                    .Select(x => x.Role.Name)
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToArray(),

            IsActive =
                user.IsActive,

            EmployeeIsActive =
                user.Employee.IsActive,

            LastLoginAt =
                user.LastLoginAt,

            CreatedAt =
                user.CreatedAt,

            UpdatedAt =
                user.UpdatedAt
        };
    }
}