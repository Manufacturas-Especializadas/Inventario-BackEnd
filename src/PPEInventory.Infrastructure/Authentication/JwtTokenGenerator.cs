using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Application.Common.Constants;

namespace PPEInventory.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenGenerator(
        IOptions<JwtSettings> options,
        IDateTimeProvider dateTimeProvider)
    {
        _settings = options.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public JwtTokenResult GenerateToken(
        User user,
        IReadOnlyCollection<string> roles)
    {
        var now = _dateTimeProvider.UtcNow;

        var expiresAt =
            now.AddMinutes(_settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new(
                AppClaimTypes.UserId,
                user.Id.ToString()),

            new(
                AppClaimTypes.EmployeeId,
                user.EmployeeId.ToString()),

            new(
                AppClaimTypes.EmployeeNumber,
                user.Employee.EmployeeNumber),

            new(
                ClaimTypes.Name,
                user.Employee.Name),

            new(
                AppClaimTypes.Username,
                user.Username)
        };

        claims.AddRange(
            roles.Select(role =>
                new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _settings.SecretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return new JwtTokenResult(
            tokenString,
            expiresAt);
    }
}