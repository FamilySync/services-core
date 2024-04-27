using FamilySync.Core.Example.Configurations;
using FamilySync.Core.Example.Services;
using FamilySync.Core.Helpers;
using Mapster;

namespace FamilySync.Core.Example;

public class Configuration : ServiceConfiguration
{
    public override void Configure(IApplicationBuilder app)
    {
    }

    public override void ConfigureMapper(TypeAdapterConfig config)
    {
        MapsterConfiguration.Configure();
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IExampleService, ExampleService>();
    }
}