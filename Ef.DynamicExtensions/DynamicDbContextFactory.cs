using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BindingFlags = System.Reflection.BindingFlags;

namespace Ef.DynamicExtensions;

public class DynamicDbContextFactory<TContext> : IDynamicDbContextFactory<TContext> where TContext : DbContext
{
    private readonly ConcurrentDictionary<string, DbContextOptions<TContext>> _optionsMap = new();
    private readonly Func<string, DbContextOptions<TContext>> _optionsFactory;
    private readonly IConfiguration _configuration;

    public DynamicDbContextFactory(Func<string, DbContextOptions<TContext>> optionsFactory,
        IConfiguration configuration)
    {
        if (typeof(TContext).GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(DbContextOptions<TContext>) },
                null) == null
            || !typeof(IDynamicDbContext<TContext>).IsAssignableFrom(typeof(TContext)))
        {
            throw new ArgumentException(
                $"Type '{typeof(TContext).Name}' does not have a constructor with parameter '{nameof(DbContextOptions<TContext>)}' or does not implement '{nameof(IDynamicDbContext<TContext>)}'.");
        }

        _optionsFactory = optionsFactory;
        _configuration = configuration;
    }

    public TContext CreateDbContext(string connectionStringName)
    {
        
        if (!_optionsMap.TryGetValue(connectionStringName, out DbContextOptions<TContext> options))
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            options = _optionsFactory.Invoke(connectionString);
            _optionsMap.TryAdd(connectionStringName, options);
        }

        if (typeof(IDynamicDbContext<TContext>).IsAssignableFrom(typeof(TContext)))
        {
            return (TContext)typeof(TContext).GetMethod("GetContext").Invoke(null,new object[] { options });
        }
        
        var constructor = typeof(TContext).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
            [typeof(DbContextOptions<TContext>)], null);

        return (TContext)constructor.Invoke(new object[] { options });
    }
}
