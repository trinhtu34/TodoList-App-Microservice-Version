using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace Infrastructure.Persistence.Data;

public class GroupServiceDbContextFactory : IDesignTimeDbContextFactory<GroupServiceDbContext>
{
    public GroupServiceDbContext CreateDbContext(string[] args)
    {
        // Load .env từ Presentation project
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Presentation", ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<GroupServiceDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        );

        return new GroupServiceDbContext(optionsBuilder.Options);
    }
}
