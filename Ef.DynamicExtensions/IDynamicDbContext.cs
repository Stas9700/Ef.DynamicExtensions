using Microsoft.EntityFrameworkCore;

namespace Ef.DynamicExtensions;

public interface IDynamicDbContext<TContext> where TContext : DbContext
{
    static abstract TContext? GetContext(DbContextOptions<TContext> options);
}