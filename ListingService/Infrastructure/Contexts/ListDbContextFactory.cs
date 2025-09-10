using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Contexts;

public class ListDbContextFactory : IDesignTimeDbContextFactory<ListDbContext>
{
    public ListDbContext CreateDbContext(string[] args)
    {
        string projectPath = Path.Combine(Directory.GetCurrentDirectory(), "../Web");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ListDbContext>();

        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.UseNetTopologySuite()
        );

        return new ListDbContext(optionsBuilder.Options);
    }
}
