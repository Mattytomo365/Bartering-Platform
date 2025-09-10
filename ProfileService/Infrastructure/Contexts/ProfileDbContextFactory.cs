using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Contexts;

public class ProfileDbContextFactory : IDesignTimeDbContextFactory<ProfileDbContext>
{
    public ProfileDbContext CreateDbContext(string[] args)
    {
        string projectPath = Path.Combine(Directory.GetCurrentDirectory(), "../Web");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ProfileDbContext>();

        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")
        );

        return new ProfileDbContext(optionsBuilder.Options);
    }
}
