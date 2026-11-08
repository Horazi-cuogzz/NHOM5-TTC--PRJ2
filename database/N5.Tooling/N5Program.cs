using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;

if (args.Length != 1 || args[0] != "initialize")
{
    Console.WriteLine("Local database initialization: dotnet run --project database/N5.Tooling/N5.Database.Tooling.csproj -- initialize");
    return;
}

var configuration = N5DesignTimeContextFactory.CreateConfiguration();
var connection = configuration.GetConnectionString("DefaultConnection") ?? N5DesignTimeContextFactory.LocalConnectionString;
var parsed = new SqlConnectionStringBuilder(connection);
string[] allowedServers = [@".\SQLEXPRESS", @"localhost\SQLEXPRESS", @"(local)\SQLEXPRESS", $@"{Environment.MachineName}\SQLEXPRESS"];
if (!string.Equals(parsed.InitialCatalog, "N5RecruitHub", StringComparison.OrdinalIgnoreCase)
    || !allowedServers.Contains(parsed.DataSource, StringComparer.OrdinalIgnoreCase) || !parsed.IntegratedSecurity)
    throw new InvalidOperationException("Công cụ này chỉ khởi tạo N5RecruitHub trên SQL Express cục bộ bằng Windows auth.");

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    EnvironmentName = Environments.Development,
    Args = []
});
builder.Configuration.Sources.Clear();
builder.Configuration.AddConfiguration(configuration).AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Database:Initialize"] = "true",
    ["Logging:LogLevel:Default"] = "Warning"
});
builder.Services.AddDbContext<N5RecruitHubContext>(options => options.UseSqlServer(connection));
builder.Services.AddIdentityCore<N5AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<N5RecruitHubContext>();
using var host = builder.Build();
await N5DbSeeder.SeedAsync(host.Services, builder.Configuration);
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<N5RecruitHubContext>();
Console.WriteLine($"N5RecruitHub ready. Users={await db.Users.CountAsync()}, companies={await db.Companies.CountAsync()}, profiles={await db.CandidateProfiles.CountAsync()}, skills={await db.Skills.CountAsync()}, jobs={await db.JobPostings.CountAsync()}, applications={await db.Applications.CountAsync()}.");
