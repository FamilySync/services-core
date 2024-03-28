using System.Reflection;
using System.Runtime.Loader;
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
        
        foreach (var asm in entryAssembly.GetReferencedAssemblies())
        {
            try
            {
                AssemblyLoadContext.Default.LoadFromAssemblyName(asm);
            }
            catch
            {
                continue;
            }
        }

        var plugins = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(o => !o.IsDynamic)
            .Where(o => o.FullName is string fn && allowedPluginAssemblyPrefix.Any(k => fn.StartsWith(k)))
            .SelectMany(o => o.GetExportedTypes())
            .Where(o => !o.IsAbstract && o.IsSubclassOf(typeof(ServiceConfiguration)))
            .Distinct()
            .Select(o => (ServiceConfiguration)Activator.CreateInstance(o)!)
            .Where(o => o is not null)
            .ToList();

        if (PluginConfigurations.Any())
        {
            PluginConfigurations.Clear();
        }

        PluginConfigurations.AddRange(plugins);
    }
    public virtual void Configure(IApplicationBuilder app)
    {
        app.UseServiceCore();
        
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