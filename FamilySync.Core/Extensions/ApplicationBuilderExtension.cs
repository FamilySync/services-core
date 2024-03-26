using FamilySync.Core.Helpers.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FamilySync.Core.Extensions;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder InitializeApplication(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices
            .GetRequiredService<IOptions<IncludeSettings>>().Value;
        
        if(settings.Mvc)
        {
            app.UseResponseCaching();
            app.UseRouting();
            app.UseHttpsRedirection();
            
            app.UseEndpoints(options =>
            {
                options.MapControllers();
            });
        }
        
        if(settings.Swagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}