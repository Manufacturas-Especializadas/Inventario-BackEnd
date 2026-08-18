using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PPEInventory.Api.Authorization;
using PPEInventory.Api.Middleware;
using PPEInventory.Api.OpenApi;
using PPEInventory.Application;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Interfaces;
using PPEInventory.Infrastructure;
using PPEInventory.Infrastructure.Authentication;
using System.Security.Claims;
using System.Text;
using PPEInventory.Api.Configuration;
using MediatR;
using Microsoft.Extensions.Options;
using PPEInventory.Api.Models;
using PPEInventory.Application.Features.Users.Commands.Create;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<BootstrapAdminSettings>()
    .Bind(
        builder.Configuration.GetSection(
            BootstrapAdminSettings.SectionName))
    .Validate(
        settings =>
            !settings.Enabled ||
            !string.IsNullOrWhiteSpace(
                settings.Key),
        "BootstrapAdmin:Key is required when bootstrap is enabled.")
    .ValidateOnStart();


const string FrontendCorsPolicy =
    "FrontendCorsPolicy";




builder.Services.AddApplication();



var connectionString =
    builder.Configuration
        .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddInfrastructure(
    connectionString);



builder.Services.AddControllers();



builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();

    options.AddOperationTransformer<
        AuthOperationTransformer>();
});



builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();



builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();



builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(
        JwtSettings.SectionName));

var jwtSecretKey =
    builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException(
        "JWT SecretKey was not found.");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer was not found.");

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT Audience was not found.");



builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,

                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSecretKey)),

                RoleClaimType =
                    ClaimTypes.Role,

                NameClaimType =
                    ClaimTypes.Name,

                ClockSkew =
                    TimeSpan.FromMinutes(1)
            };
    });



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.Administrator,
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                AppRoles.Administrator);
        });

    options.AddPolicy(
        AuthorizationPolicies.Production,
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                AppRoles.Administrator,
                AppRoles.Production);
        });

    options.AddPolicy(
        AuthorizationPolicies.Warehouse,
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                AppRoles.Administrator,
                AppRoles.Warehouse);
        });

    options.AddPolicy(
        AuthorizationPolicies.Viewer,
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                AppRoles.Administrator,
                AppRoles.Production,
                AppRoles.Warehouse,
                AppRoles.Viewer);
        });
});



var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ?? Array.Empty<string>();

if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "No CORS allowed origins were configured.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        FrontendCorsPolicy,
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.AddHealthChecks();


var app = builder.Build();



app.UseExceptionHandler();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "PPE Inventory API v1");
    });
}



app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.MapHealthChecks(
    "/health");

var bootstrapSettings =
    app.Services
        .GetRequiredService<
            IOptions<BootstrapAdminSettings>>()
        .Value;

if (bootstrapSettings.Enabled)
{
    app.MapPost(
        "/api/setup/bootstrap-admin",
        async (
            BootstrapAdminRequest request,
            IUserRepository userRepository,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (await userRepository.AnyAsync(
                cancellationToken))
            {
                return Results.Conflict(
                    new
                    {
                        message =
                            "Bootstrap is no longer available because the system already contains users."
                    });
            }

            if (!IsValidBootstrapKey(
                    request.BootstrapKey,
                    bootstrapSettings.Key!))
            {
                return Results.Unauthorized();
            }

            var command =
                new CreateUserCommand(
                    request.EmployeeNumber,
                    request.Username,
                    request.Password,
                    new[]
                    {
                        AppRoles.Administrator
                    });

            var userId =
                await mediator.Send(
                    command,
                    cancellationToken);

            return Results.Ok(
                new
                {
                    userId,
                    message =
                        "Initial administrator created successfully. Disable BootstrapAdmin immediately."
                });
        })
        .AllowAnonymous();
}


app.Run();


static bool IsValidBootstrapKey(
    string providedKey,
    string expectedKey)
{
    if (string.IsNullOrWhiteSpace(
            providedKey) ||
        string.IsNullOrWhiteSpace(
            expectedKey))
    {
        return false;
    }

    var providedHash =
        SHA256.HashData(
            Encoding.UTF8.GetBytes(
                providedKey));

    var expectedHash =
        SHA256.HashData(
            Encoding.UTF8.GetBytes(
                expectedKey));

    return CryptographicOperations
        .FixedTimeEquals(
            providedHash,
            expectedHash);
}