using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class SearchDbContextFactory : IDesignTimeDbContextFactory<SearchDbContext>
{
    public SearchDbContext CreateDbContext(string[] args)
    {
        string projectPath = Path.Combine(Directory.GetCurrentDirectory(), "../Web");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SearchDbContext>();

        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.UseNetTopologySuite()
        );

        return new SearchDbContext(optionsBuilder.Options);
    }
}
