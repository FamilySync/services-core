using FamilySync.Core.Helpers.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FamilySync.Core.Persistence.Filters;

public interface IMigrationFilter
{
    public Task ApplyPending();
    public Task Verify();
}

public class MigrationFilter<TContext> : IMigrationFilter where TContext : DbContext
{
    private readonly TContext _context;

    public MigrationFilter(TContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets any pending migrations and tries to apply them to the database
    /// </summary>
    /// <exception cref="FileNotFoundException">If a migration exists but it cannot find the associated sql script</exception>
    public async Task ApplyPending()
    {
        
        // Get any pending migrations
        var pending = await _context.Database.GetPendingMigrationsAsync();

        if (!pending.Any())
        {
            // TODO: Log implementation: "No pending migrations for {ContextType}", typeof(TContext).Name
            return;
        }
        
        // TODO: Log Implementation: "{Count} migrations is being applied for {ContextType}", pending.Count(), typeof(TContext).Name
        
        // Make it possible for the database to timeout and stop
        _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));

        // Iterate through all pending migrations and get their associated sql script and apply it to the database
        foreach (var migrations in pending)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Migrations/Scripts", $"{migrations}.sql");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Missing migration script for {migrations}", path);
            }

            var raw = await File.ReadAllTextAsync(path);

            await _context.Database.ExecuteSqlRawAsync(raw);
        }
    }

    /// <summary>
    /// Verify that all pending migrations has been successfully applied to the database
    /// </summary>
    public async Task Verify()
    {
        var pending = await _context.Database.GetPendingMigrationsAsync();

        if (pending.Any())
        {
            // TODO: Log Implementation: "{Count} pending migrations have not been applied", pending.Count()
            throw new DatabaseException($"{pending.Count()} pending migrations have not been applied");
        }
    }
}