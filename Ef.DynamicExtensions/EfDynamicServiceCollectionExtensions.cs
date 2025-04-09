using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ef.DynamicExtensions;

public static class EfDynamicServiceCollectionExtensions
{
    public static IServiceCollection AddDynamicDbContextFactory<TContext>(this IServiceCollection serviceCollection,Func<string,DbContextOptions<TContext>> optionsFactory = null)
        where TContext : DbContext
    {
        serviceCollection.AddSingleton<IDynamicDbContextFactory<TContext>, DynamicDbContextFactory<TContext>>(provider =>
        {
            return new DynamicDbContextFactory<TContext>(optionsFactory, provider.GetRequiredService<IConfiguration>());
        });
        return serviceCollection;
    }
}