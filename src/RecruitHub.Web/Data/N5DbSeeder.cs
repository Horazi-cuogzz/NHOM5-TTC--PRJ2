using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecruitHub.Web.Models;

namespace RecruitHub.Web.Data;

public static class N5DbSeeder
{
    private const string SeedClaim = "N5RecruitHub:DemoSeed";
    private const string SeedVersion = "v1";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
            throw new InvalidOperationException("Dữ liệu demo N5 chỉ được khởi tạo trong môi trường Development.");
        if (!configuration.GetValue<bool>("Database:Initialize")) return;

        var db = scope.ServiceProvider.GetRequiredService<N5RecruitHubContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<N5AppUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        // Serialize concurrent local initialization; all demo changes commit together.
        await db.Database.ExecuteSqlRawAsync("""
            DECLARE @result int;
            EXEC @result = sys.sp_getapplock @Resource=N'N5RecruitHub:DemoSeed',
                @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=60000;
            IF @result < 0 THROW 51000, 'Cannot obtain N5 demo seed lock.', 1;
            """);

        if (await db.UserClaims.AnyAsync(x => x.ClaimType == SeedClaim && x.ClaimValue == SeedVersion))
        {
            await transaction.CommitAsync();
            return;
        }

        var password = configuration["Seed:DemoPassword"];
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Thiếu Seed:DemoPassword. Đặt bằng user-secrets hoặc Seed__DemoPassword trước khi khởi tạo dữ liệu demo.");

        foreach (var name in new[] { "Candidate", "Employer", "Admin" })
        {
            if (!await roles.RoleExistsAsync(name))
                RequireSuccess(await roles.CreateAsync(new IdentityRole(name)), $"Tạo vai trò {name}");
        }

        var admin = await EnsureUserAsync(users, "n5-demo-admin", "admin@recruithub.example", "Quản trị Demo N5", "Admin", password);
        var employer1 = await EnsureUserAsync(users, "n5-demo-employer-1", "employer1@recruithub.example", "Nhà tuyển dụng Demo 1", "Employer", password);
        var employer2 = await EnsureUserAsync(users, "n5-demo-employer-2", "employer2@recruithub.example", "Nhà tuyển dụng Demo 2", "Employer", password);
        var candidates = new List<N5AppUser>();
        for (var i = 1; i <= 5; i++)
            candidates.Add(await EnsureUserAsync(users, $"n5-demo-candidate-{i}", $"candidate{i}@recruithub.example", $"Ứng viên Demo {i}", "Candidate", password));

        string[] skillNames = ["C", "C#", "C++", "SQL Server", "Git", "HTML", "CSS", "JavaScript", "React", "Python", "Django", "ASP.NET Core", "Entity Framework Core", "Java", "Spring Boot", "MySQL", "Docker", "TypeScript", "Node.js", "jQuery"];
        var skills = new Dictionary<string, N5Skill>(StringComparer.Ordinal);
        foreach (var name in skillNames)
        {
            var normalized = N5Skill.NormalizeName(name);
            var skill = await db.Skills.SingleOrDefaultAsync(x => x.NormalizedName == normalized);
            if (skill is null)
            {
                skill = new N5Skill { Name = name };
                db.Skills.Add(skill);
            }
            skills[name] = skill;
        }

        var company1 = await EnsureCompanyAsync(db, employer1, "N5 Sao Việt Software Demo", "Hà Nội");
        var company2 = await EnsureCompanyAsync(db, employer2, "N5 Mây Xanh Tech Demo", "TP. Hồ Chí Minh");
        string[][] candidateSkills =
        [
            ["C#", "SQL Server", "HTML", "CSS"],
            ["HTML", "CSS", "JavaScript", "React", "TypeScript", "Git"],
            ["Python", "Django", "Git", "Docker"],
            ["Java", "Spring Boot", "MySQL", "Git"],
            ["C#", "SQL Server", "Git", "ASP.NET Core", "Entity Framework Core"]
        ];
        var profiles = new List<N5CandidateProfile>();
        for (var i = 0; i < candidates.Count; i++)
        {
            var user = candidates[i];
            var profile = await db.CandidateProfiles.SingleOrDefaultAsync(x => x.UserId == user.Id);
            if (profile is null)
            {
                profile = new N5CandidateProfile
                {
                    User = user,
                    Phone = $"000000000{i + 1}",
                    Location = i % 2 == 0 ? "Hà Nội" : "TP. Hồ Chí Minh",
                    Introduction = $"Hồ sơ giả lập của ứng viên Demo {i + 1}, mong muốn làm việc trong lĩnh vực phát triển phần mềm.",
                    Education = "Chương trình Công nghệ thông tin (dữ liệu minh họa).",
                    Experience = "Đã thực hiện dự án học tập; thông tin này chỉ phục vụ trình diễn RecruitHub.",
                    Skills = candidateSkills[i].Select(name => new N5CandidateSkill { Skill = skills[name] }).ToList()
                };
                db.CandidateProfiles.Add(profile);
            }
            profiles.Add(profile);
        }

        var now = DateTimeOffset.UtcNow;
        var jobs = new List<N5JobPosting>
        {
            CreateJob(company1, "Lập trình viên .NET Demo", "Hà Nội", "15–25 triệu đồng", now.AddDays(-12), now.AddDays(30), N5JobStatus.Published, skills, "C#", "SQL Server", "Git"),
            CreateJob(company2, "Lập trình viên Frontend Demo", "TP. Hồ Chí Minh", "12–22 triệu đồng", now.AddDays(-11), now.AddDays(25), N5JobStatus.Published, skills, "HTML", "CSS", "JavaScript", "React"),
            CreateJob(company2, "Lập trình viên Python Demo", "Làm việc từ xa", "Thỏa thuận", now.AddDays(-10), now.AddDays(20), N5JobStatus.Published, skills, "Python", "Django"),
            CreateJob(company1, "Lập trình viên ASP.NET Core Demo", "Hà Nội", "18–28 triệu đồng", now.AddDays(-9), now.AddDays(35), N5JobStatus.Published, skills, "C#", "ASP.NET Core", "Entity Framework Core", "SQL Server"),
            CreateJob(company2, "Lập trình viên React TypeScript Demo", "TP. Hồ Chí Minh", "18–30 triệu đồng", now.AddDays(-8), now.AddDays(40), N5JobStatus.Published, skills, "React", "TypeScript", "JavaScript", "Git"),
            CreateJob(company1, "Lập trình viên Java Demo", "Đà Nẵng", "16–26 triệu đồng", now.AddDays(-7), now.AddDays(30), N5JobStatus.Published, skills, "Java", "Spring Boot", "MySQL", "Git"),
            CreateJob(company2, "Lập trình viên Node.js Demo", "Làm việc từ xa", "Thỏa thuận", now.AddDays(-6), now.AddDays(45), N5JobStatus.Published, skills, "Node.js", "JavaScript", "MySQL", "Git"),
            CreateJob(company1, "Lập trình viên C++ Demo", "Hà Nội", "Thỏa thuận", now.AddDays(-5), now.AddDays(15), N5JobStatus.Published, skills, "C", "C++", "Git"),
            CreateJob(company1, "Thực tập ASP.NET Core Demo (nháp)", "Hà Nội", "Hỗ trợ thực tập", null, now.AddDays(60), N5JobStatus.Draft, skills, "C#", "ASP.NET Core"),
            CreateJob(company2, "Thực tập Frontend Demo (nháp)", "TP. Hồ Chí Minh", "Hỗ trợ thực tập", null, now.AddDays(60), N5JobStatus.Draft, skills, "HTML", "CSS", "JavaScript"),
            CreateJob(company1, "Lập trình viên .NET Demo (đã đóng)", "Hà Nội", "Thỏa thuận", now.AddDays(-30), now.AddDays(-3), N5JobStatus.Closed, skills, "C#", "SQL Server"),
            CreateJob(company2, "Lập trình viên Web Demo (hết hạn)", "TP. Hồ Chí Minh", "Thỏa thuận", now.AddDays(-25), now.AddDays(-1), N5JobStatus.Published, skills, "HTML", "CSS", "jQuery")
        };
        db.JobPostings.AddRange(jobs);
        // Candidate 1 has no applications so the SRS 66.67%, 50%, 0% example remains visible.
        db.Applications.AddRange(
            CreateApplication(jobs[1], profiles[1], N5ApplicationStatus.Submitted, now.AddDays(-2)),
            CreateApplication(jobs[2], profiles[2], N5ApplicationStatus.Reviewing, now.AddDays(-3)),
            CreateApplication(jobs[5], profiles[3], N5ApplicationStatus.Accepted, now.AddDays(-4)),
            CreateApplication(jobs[0], profiles[4], N5ApplicationStatus.Rejected, now.AddDays(-5)),
            CreateApplication(jobs[10], profiles[4], N5ApplicationStatus.Reviewing, now.AddDays(-10)),
            CreateApplication(jobs[11], profiles[1], N5ApplicationStatus.Submitted, now.AddDays(-10)));
        await db.SaveChangesAsync();
        RequireSuccess(await users.AddClaimAsync(admin, new Claim(SeedClaim, SeedVersion)), "Lưu mốc hoàn tất seed");
        await transaction.CommitAsync();
    }

    private static async Task<N5AppUser> EnsureUserAsync(UserManager<N5AppUser> manager,
        string id, string email, string fullName, string role, string password)
    {
        var user = await manager.FindByIdAsync(id);
        if (user is null)
        {
            if (await manager.FindByEmailAsync(email) is not null)
                throw new InvalidOperationException($"Email demo {email} đã thuộc tài khoản khác; seed dừng để giữ nguyên dữ liệu.");
            user = new N5AppUser { Id = id, UserName = email, Email = email, EmailConfirmed = true, FullName = fullName };
            RequireSuccess(await manager.CreateAsync(user, password), $"Tạo tài khoản demo {email}");
        }
        if (!await manager.IsInRoleAsync(user, role))
            RequireSuccess(await manager.AddToRoleAsync(user, role), $"Gán vai trò demo {role}");
        return user;
    }

    private static async Task<N5Company> EnsureCompanyAsync(N5RecruitHubContext db, N5AppUser employer, string name, string city)
    {
        var company = await db.Companies.SingleOrDefaultAsync(x => x.EmployerUserId == employer.Id);
        if (company is not null) return company;
        company = new N5Company
        {
            EmployerUser = employer, Name = name,
            Description = "Doanh nghiệp IT giả lập phục vụ học tập và trình diễn RecruitHub.",
            Address = $"Địa chỉ minh họa tại {city}", Website = "https://example.com"
        };
        db.Companies.Add(company);
        return company;
    }

    private static N5JobPosting CreateJob(N5Company company, string title, string location, string salary,
        DateTimeOffset? publishedAt, DateTimeOffset deadline, N5JobStatus status,
        IReadOnlyDictionary<string, N5Skill> skills, params string[] requiredSkills) => new()
        {
            Company = company, Title = title, Location = location, SalaryText = salary,
            Description = $"{title}. Tham gia phát triển và bảo trì ứng dụng; phối hợp với nhóm dự án. Đây là tin tuyển dụng giả lập dùng để minh họa chức năng.",
            PublishedAt = publishedAt, Deadline = deadline, Status = status,
            Skills = requiredSkills.Select(name => new N5JobSkill { Skill = skills[name] }).ToList()
        };

    private static N5Application CreateApplication(N5JobPosting job, N5CandidateProfile profile,
        N5ApplicationStatus status, DateTimeOffset appliedAt) => new()
        {
            JobPosting = job, CandidateProfile = profile, Status = status, AppliedAt = appliedAt,
            CoverLetter = "Đơn ứng tuyển giả lập: tôi mong muốn trao đổi thêm về vị trí và đóng góp các kỹ năng đã khai báo trong hồ sơ."
        };

    private static void RequireSuccess(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException($"{operation}: {string.Join("; ", result.Errors.Select(x => x.Description))}");
    }
}
