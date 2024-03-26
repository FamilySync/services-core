using System.Reflection;
using FamilySync.Core.Extensions;
using FamilySync.Core.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilySync.Core;

public class Startup
{
    // List all allowed assembly prefixes used to filter .. Security
    private static readonly List<string> allowedPluginAssemblyPrefix = new() { "FamilySync"};
    
     // List to store instances of ServiceConfiguration (a custom class).
    private static readonly List<ServiceConfiguration> PluginConfigurations = new();

    // Configuration property for storing application configuration.
    public IConfiguration? Configuration { get; init; }

    public Startup()
    {
        LoadPluginAssemblies();
    }
    
    /// <summary>
    /// // Loads assemblies that are referenced by the entry assembly.
    /// </summary>
    private static void LoadPluginAssemblies()
    {
        // Get the entry assembly (the main application assembly).
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null)
        {
            // throw a exception if the entry assembly is not found.
            throw new Exception("GetEntryAssembly returned null");
        }
        
        /*
         * 1. Get the current application domain.
         * 2. Get all loaded assemblies in the domain.
         * 3. Filter out dynamic assemblies.
         * 4. Get the exported types from all assemblies.
         * 5. Filter out abstract types.
         * 6. Filter for types that are subclasses of ServiceConfiguration.
         * 7. Filter types based on prefixes.
         * 8. Create instances of the selected types.
         * 9. Filter out any null instances.
         * 10. Convert the result to a List<ServiceConfiguration>.
         */
        var plugins = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ServiceConfiguration)))
            .Where(type => allowedPluginAssemblyPrefix.Any(prefix => type.FullName?.StartsWith(prefix) == true))
            .Select(type => (ServiceConfiguration?)Activator.CreateInstance(type))
            .Where(config => config != null)
            .ToList();
        
        // Clear and update the list of plugin configurations.
        PluginConfigurations.Clear();
        PluginConfigurations.AddRange(plugins!);
    }
    public virtual void Configure(IApplicationBuilder app)
    {
        app.InitializeApplication();
        
        Configure(app, PluginConfigurations);
    }
    protected virtual void Configure(IApplicationBuilder app, List<ServiceConfiguration> configurations)
    {
        foreach (var configuration in configurations)
        {
            configuration.Configuration = Configuration!;
            configuration.Configure(app);
        }
    }
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.InitializeService(Configuration!);
        
        ConfigureServices(services, PluginConfigurations);
    }
    protected virtual void ConfigureServices(IServiceCollection services, List<ServiceConfiguration> configurations)
    {
        foreach (var configuration in configurations)
        {
            configuration.Configuration = Configuration!;
            configuration.ConfigureServices(services);
        }
    }
}