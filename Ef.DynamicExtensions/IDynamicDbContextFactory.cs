using Microsoft.EntityFrameworkCore;

namespace Ef.DynamicExtensions;

public interface IDynamicDbContextFactory<TContext> where TContext : DbContext
{
    TContext CreateDbContext(string connectionStringName);
}