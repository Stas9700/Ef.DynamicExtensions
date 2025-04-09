using Ef.DynamicExtensions;
using Microsoft.EntityFrameworkCore;

namespace EfExperiments.Entities;

public class TestDbContext: DbContext, IDynamicDbContext<TestDbContext>
{
    public TestDbContext(DbContextOptions<TestDbContext> options): base(options) { }
    
    public DbSet<TestEntity> TestEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
    }

    public static TestDbContext? GetContext(DbContextOptions<TestDbContext> options)
    {
        return new TestDbContext(options);
    }
}