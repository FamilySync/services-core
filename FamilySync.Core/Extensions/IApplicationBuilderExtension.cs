using FamilySync.Core.Helpers.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace FamilySync.Core.Extensions;

public static class IApplicationBuilderExtension
{
    public static IApplicationBuilder UseServiceCore(this IApplicationBuilder applicationBuilder)
    {
        var include = applicationBuilder.ApplicationServices
            .GetRequiredService<IOptions<IncludeSettings>>().Value;

        if (include.Mvc)
        {
            applicationBuilder.UseResponseCaching();
            
            applicationBuilder.UseRouting();
        }

        if (include.Swagger)
        {
            applicationBuilder.UseSwagger();
        }

        if (include.Mvc)
        {
            applicationBuilder.UseEndpoints(options =>
            {
                options.MapControllers();
                
            });
        }

        return applicationBuilder;
    }
    
    private static void UseSwagger(this IApplicationBuilder applicationBuilder)
    {
        var options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<ServiceSettings>>().Value;
        var apiProvider = applicationBuilder.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

        applicationBuilder.UseSwagger(config =>
        {
            config.RouteTemplate = "api/swagger/{documentName}/swagger.json";

            if (!options.Debug)
            {
                config.PreSerializeFilters.Add((swagger, _) =>
                {
                    var paths = new OpenApiPaths();

                    foreach (var path in swagger.Paths)
                    {
                        paths.Add(path.Key.Replace("/api/", $"/{options.Route}/"), path.Value);
                    }

                    swagger.Paths = paths;
                });
            }
        });

        applicationBuilder.UseSwaggerUI(config =>
        {
            foreach (var description in apiProvider.ApiVersionDescriptions)
            {
                var title = $"FamilySync - {options.Name} {description.GroupName}";

                if (options.Debug)
                {
                    config.SwaggerEndpoint($"/api/swagger/{description.GroupName}/swagger.json", title);
                }
                else
                {
                    config.SwaggerEndpoint($"/{options.Route}/swagger/{description.GroupName}/swagger.json", title);
                }
            }

            config.RoutePrefix = "api/swagger";
        });
    }


}