﻿using FamilySync.Core.Helpers.Settings;
using FamilySync.Core.Settings;
using FastExpressionCompiler;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FamilySync.Core.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection InitializeService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceSettings>(configuration.GetRequiredSection("Settings:Service"));
        services.Configure<IncludeSettings>(configuration.GetSection("Settings:Include"));

        var settings = configuration
            .GetRequiredSection("Settings")
            .Get<ConfigurationSettings>()!;

        if (settings.Include.Versioning)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = false;
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new(1, 0);
            });
        
            services.AddVersionedApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
                options.GroupNameFormat = "'v'VVV";
            });
        }

        if (settings.Include.Mvc)
        {
            var mvcBuilder = services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; });

            // Obtain all assemblies and add them to mvc so it can discover what it needs .. like controllers
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
        }

        if (settings.Include.Swagger)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigurationSettings>();
        
            services.AddSwaggerGen(options =>
            {
                options.UseAllOfToExtendReferenceSchemas();
                options.CustomOperationIds(o => $"{o.ActionDescriptor.RouteValues["action"]}");
        
                const string securityDefinition = "Bearer";
        
                var scheme = new OpenApiSecurityScheme
                {
                    Description = $"Authorization header for JWT using {securityDefinition} scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = $"{securityDefinition}",
                    Reference = new OpenApiReference
                    {
                        Id = $"{securityDefinition}",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                
                options.AddSecurityDefinition(securityDefinition, scheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {scheme, new List<string>()}
                });
            });
        
            services.AddEndpointsApiExplorer();
        }

        if (settings.Include.Mapper)
        {
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileFast();
            TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = false;

            services.AddSingleton<IMapper, ServiceMapper>();
        }

        return services;
    }
}