using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PPEInventory.Application.Common.Notifications;
using PPEInventory.Application.Interfaces;
using PPEInventory.Infrastructure;
using PPEInventory.Infrastructure.Notifications;

internal static class RealTeamsCheck
{
    public static async Task<int> RunAsync()
    {
        try
        {
            var apiDirectory = FindApiDirectory();
            // Read the API's actual ID instead of copying it or reading/printing secret values.
            var project = XDocument.Load(Path.Combine(apiDirectory, "PPEInventory.Api.csproj"));
            var secretsId = project.Descendants("UserSecretsId").Single().Value;
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Development";

            // Same configuration sources/precedence as the API, with secrets explicitly loaded
            // for this local tool. Building configuration does not start the API or a web host.
            using var configuration = new ConfigurationManager();
            configuration
                .SetBasePath(apiDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddUserSecrets(secretsId, reloadOnChange: false)
                .AddEnvironmentVariables();

            var services = new ServiceCollection();
            // No logging providers: console output is limited to the fixed messages below.
            services.AddLogging(builder => builder.ClearProviders());
            services.Configure<TeamsWebhookSettings>(configuration.GetSection(TeamsWebhookSettings.SectionName));
            // Reuse production DI. No database service is resolved or connection opened.
            services.AddInfrastructure(configuration.GetConnectionString("DefaultConnection") ?? string.Empty);
            using var provider = services.BuildServiceProvider();
            var sender = provider.GetRequiredService<ITeamsWebhookSender>();
            var message = new TeamsNotificationMessage(
                TeamsNotificationDestination.Warehouse,
                "Prueba PPE Inventory",
                "La integración de notificaciones de PPE Inventory con Microsoft Teams funciona correctamente.");

            Console.WriteLine("Enviando notificación de prueba a Warehouse...");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            // Exactly one invocation, with the real registered transport and no retry loop.
            await sender.SendAsync(message, timeout.Token);
            Console.WriteLine("Notificación enviada correctamente.");
            return 0;
        }
        catch (HttpRequestException exception)
        {
            Console.Error.WriteLine("Error enviando notificación a Teams.");
            if (exception.StatusCode is { } status)
                Console.Error.WriteLine($"HTTP status: {(int)status}");
            return 1;
        }
        catch (Exception)
        {
            // Configuration/parsing/transport errors can carry secrets; never print them.
            Console.Error.WriteLine("Error enviando notificación a Teams.");
            return 1;
        }
    }

    private static string FindApiDirectory()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "src", "PPEInventory.Api");
            if (File.Exists(Path.Combine(candidate, "PPEInventory.Api.csproj")))
                return candidate;
        }

        throw new InvalidOperationException("API project directory was not found.");
    }
}
