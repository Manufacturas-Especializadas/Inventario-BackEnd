using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PPEInventory.Api.OpenApi;

public sealed class BearerSecuritySchemeTransformer
    : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider
        _authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider =
            authenticationSchemeProvider;
    }

    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes =
            await _authenticationSchemeProvider
                .GetAllSchemesAsync();

        if (!authenticationSchemes.Any(
            x => x.Name == "Bearer"))
        {
            return;
        }

        var securitySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                }
            };

        document.Components ??=
            new OpenApiComponents();

        document.Components.SecuritySchemes =
            securitySchemes;
    }
}