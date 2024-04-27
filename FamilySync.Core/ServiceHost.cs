using FamilySync.Core.Persistence.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FamilySync.Core;

public static class ServiceHost<TStartup> where TStartup : Startup, new()
{
    public static int Run(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        // TODO: Implement logging
        
        try
        {
            var appBuilder = WebApplication.CreateBuilder(args);


            var startup = new TStartup()
            {
                Configuration = appBuilder.Configuration
            };

            startup.InitializeServices(appBuilder.Services);

            var app = appBuilder.Build();
            
            startup.ConfigureApp(app);

            switch (args.Any() ? args[0] : string.Empty)
            {
                case "migrate":
                    ApplyMigrations(app).Wait();
                    break;
                
                // TODO: Implement exporting of schemas. This should dynamically export based on if using swagger or something else like GraphQL
                // case "docs":
                //     ExportSchemas(app);
                //     break;
                
                default:
                    app.Run();
                    break;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex} An fatal error occurred while executing host");
            throw;
        }

        return 0;
    }

    private static void ExportSchemas(IHost app)
    {
        throw new NotImplementedException();
    }

    private static async Task ApplyMigrations(IHost app)
    {
        var provider = app.Services.GetRequiredService<IServiceProvider>();

        using var scope = provider.CreateScope();

        var migrations = scope.ServiceProvider.GetService<IEnumerable<IMigrationFilter>>();
        
        if (migrations?.Any() ?? false)
        {
            foreach (var migration in migrations)
            {
                await migration.ApplyPending();
            }
        }
    }
}

public static class ServiceHost
{
    public static int Run(string[] args)
    {
        return ServiceHost<Startup>.Run(args);
    }
}