using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilySync.Core.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMySqlContext<TContext>(this IServiceCollection services, string databaseName,
        IConfiguration configuration) where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString("MySQL");
        var version = new MySqlServerVersion("8.0.26");

        services.AddDbContext<DbContext, TContext>(options =>
        {
            options.UseMySql($"{connectionString};Database={databaseName}", version, options =>
            {
                options
                    .EnableRetryOnFailure();
            });
        });
        
        return services;
    }
    public static IServiceCollection AddPostgreSqlContext<TContext>(this IServiceCollection services, string dbName,
        IConfiguration configuration) where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        services.AddDbContext<DbContext, TContext>(options =>
        {
            options.UseNpgsql(connectionString, actions =>
            {
                actions.MigrationsAssembly(typeof(TContext).Assembly.FullName);
            });
        });
        
        return services;
    }
}