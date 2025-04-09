// See https://aka.ms/new-console-template for more information

using Ef.DynamicExtensions;
using EfExperiments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        
        IServiceCollection services = new ServiceCollection();
        services.AddDynamicDbContextFactory<TestDbContext>(connectionString =>
        {
            var builder = new DbContextOptionsBuilder<TestDbContext>();
            builder.UseSqlite(connectionString);
            return builder.Options;
        });
        services.AddSingleton<IConfiguration>(configuration);

        var dbContextFactory = services.BuildServiceProvider().GetService<IDynamicDbContextFactory<TestDbContext>>();
        
        try
        {
            using (TestDbContext dbContext = dbContextFactory?.CreateDbContext("TestDbContext1"))
            {
                Console.WriteLine("Уже накатываем миграции.");
                var t = await dbContext?.Database.GetAppliedMigrationsAsync()!;
                await dbContext?.Database.MigrateAsync()!;
            }
            Console.WriteLine("База данных обновлена");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
