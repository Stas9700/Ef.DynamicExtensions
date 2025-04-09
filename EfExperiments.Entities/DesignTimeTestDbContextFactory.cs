using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EfExperiments.Entities;

public class DesignTimeTestDbContextFactory: IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>();
        options.EnableDetailedErrors();
        const string dbName = "cache.db";
        var defaultPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()
            ?.Location);
        Console.WriteLine("Default:" + defaultPath);
        var fullPath =
            Path.Combine(defaultPath ?? throw new InvalidOperationException(nameof(defaultPath)), dbName);
        Console.WriteLine("Full:" + fullPath);
        options.UseSqlite($"Data Source={fullPath}");
        
        return new TestDbContext(options.Options);
    }
}