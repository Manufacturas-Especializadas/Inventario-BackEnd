using System.Security.Claims;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Api.Authorization;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? CurrentUser =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        CurrentUser?.Identity?.IsAuthenticated == true;

    public int? UserId =>
        GetIntClaim(AppClaimTypes.UserId);

    public int? EmployeeId =>
        GetIntClaim(AppClaimTypes.EmployeeId);

    public string? EmployeeNumber =>
        GetClaim(AppClaimTypes.EmployeeNumber);

    public string? Name =>
        GetClaim(ClaimTypes.Name);

    public string? Username =>
        GetClaim(AppClaimTypes.Username);

    public IReadOnlyCollection<string> Roles =>
        CurrentUser?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct()
            .ToArray()
        ?? Array.Empty<string>();

    public bool IsInRole(string role)
    {
        return CurrentUser?.IsInRole(role) == true;
    }

    private string? GetClaim(string claimType)
    {
        return CurrentUser?
            .FindFirst(claimType)?
            .Value;
    }

    private int? GetIntClaim(string claimType)
    {
        var value = GetClaim(claimType);

        return int.TryParse(value, out var result)
            ? result
            : null;
    }
}