using FamilySync.Core.Example.Services;
using FamilySync.Core.Helpers;

namespace FamilySync.Core.Example;

public class Configuration : ServiceConfiguration
{
    public override void Configure(IApplicationBuilder app)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IExampleService, ExampleService>();
    }
}