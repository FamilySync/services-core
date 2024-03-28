using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace FamilySync.Core.Helpers;

public abstract class ServiceConfiguration
{
    public IConfiguration Configuration { get; set; } = default!;

    public abstract void Configure(IApplicationBuilder app);

    public virtual void ConfigureMapper(TypeAdapterConfig config)
    {
    }

    public abstract void ConfigureServices(IServiceCollection services);
}