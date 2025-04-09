using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BindingFlags = System.Reflection.BindingFlags;

namespace Ef.DynamicExtensions;

public class DynamicDbContextFactory<TContext>: IDynamicDbContextFactory<TContext> where TContext : DbContext
{
    private readonly ConcurrentDictionary<string, DbContextOptions<TContext>> _optionsMap = new();
    private readonly Func<string, DbContextOptions<TContext>> _optionsFactory;
    private readonly IConfiguration _configuration;
    
    public DynamicDbContextFactory(Func<string,DbContextOptions<TContext>> optionsFactory, IConfiguration configuration)
    {
        var constr =
            typeof(TContext).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [typeof(DbContextOptions<TContext>)], null);
        if(typeof(TContext).GetConstructor(BindingFlags.Public | BindingFlags.Instance,null, [typeof(DbContextOptions<TContext>)], null) == null)
            throw new ArgumentException($"Type '{typeof(TContext).Name}' does not have a constructor with parameter '{nameof(DbContextOptions<TContext>)}'.");
        
        _optionsFactory = optionsFactory;
        _configuration = configuration;
    }
    
    public TContext CreateDbContext(string connectionStringName)
    {
        var constructor = typeof(TContext).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [typeof(DbContextOptions<TContext>)], null);
        if (_optionsMap.TryGetValue(connectionStringName, out DbContextOptions<TContext> options))
        {
            return (TContext)constructor.Invoke(new object[] { options });
        }
        var connectionString = _configuration.GetConnectionString(connectionStringName);
        var newOptions = _optionsFactory.Invoke(connectionString);
        _optionsMap.TryAdd(connectionStringName, newOptions);
        return (TContext)constructor.Invoke(new object[] { newOptions });
    }
}