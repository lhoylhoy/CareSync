using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CareSync.Infrastructure.Data;

public class CareSyncDbContextFactory : IDesignTimeDbContextFactory<CareSyncDbContext>
{
    public CareSyncDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CareSync.API"))
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CareSyncDbContext>();
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            options => options.MigrationsAssembly(typeof(CareSyncDbContext).Assembly.FullName));

        return new CareSyncDbContext(optionsBuilder.Options);
    }
}
