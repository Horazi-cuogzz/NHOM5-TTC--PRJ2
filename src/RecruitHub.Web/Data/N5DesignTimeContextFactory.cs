using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RecruitHub.Web.Data;

public class N5DesignTimeContextFactory : IDesignTimeDbContextFactory<N5RecruitHubContext>
{
    public const string LocalConnectionString =
        @"Server=.\SQLEXPRESS;Database=N5RecruitHub;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

    public N5RecruitHubContext CreateDbContext(string[] args)
    {
        var configuration = CreateConfiguration();
        var options = new DbContextOptionsBuilder<N5RecruitHubContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? LocalConnectionString)
            .Options;
        return new N5RecruitHubContext(options);
    }

    public static IConfigurationRoot CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory()); directory is not null; directory = directory.Parent)
        {
            var webPath = File.Exists(Path.Combine(directory.FullName, "N5RecruitHub.Web.csproj"))
                ? directory.FullName : Path.Combine(directory.FullName, "src", "RecruitHub.Web");
            if (!File.Exists(Path.Combine(webPath, "N5RecruitHub.Web.csproj"))) continue;
            builder.AddJsonFile(Path.Combine(webPath, "appsettings.json"), optional: true)
                .AddJsonFile(Path.Combine(webPath, "appsettings.Development.json"), optional: true);
            break;
        }

        return builder.AddUserSecrets<N5RecruitHubContext>(optional: true).AddEnvironmentVariables().Build();
    }
}
