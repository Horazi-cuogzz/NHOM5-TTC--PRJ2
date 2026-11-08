using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Models;

namespace RecruitHub.Web.Data;

public class N5RecruitHubContext(DbContextOptions<N5RecruitHubContext> options)
    : IdentityDbContext<N5AppUser>(options)
{
    public DbSet<N5CandidateProfile> CandidateProfiles => Set<N5CandidateProfile>();
    public DbSet<N5Company> Companies => Set<N5Company>();
    public DbSet<N5Skill> Skills => Set<N5Skill>();
    public DbSet<N5CandidateSkill> CandidateSkills => Set<N5CandidateSkill>();
    public DbSet<N5JobPosting> JobPostings => Set<N5JobPosting>();
    public DbSet<N5JobSkill> JobSkills => Set<N5JobSkill>();
    public DbSet<N5Application> Applications => Set<N5Application>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<N5AppUser>().Property(x => x.FullName).HasMaxLength(100).IsRequired();

        builder.Entity<N5CandidateProfile>(entity =>
        {
            entity.ToTable("N5CandidateProfiles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(30);
            entity.Property(x => x.Location).HasMaxLength(200);
            entity.Property(x => x.Introduction).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Education).HasMaxLength(4000);
            entity.Property(x => x.Experience).HasMaxLength(4000);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.HasOne(x => x.User).WithOne(x => x.CandidateProfile)
                .HasForeignKey<N5CandidateProfile>(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<N5Company>(entity =>
        {
            entity.ToTable("N5Companies");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EmployerUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.Address).HasMaxLength(500);
            entity.Property(x => x.Website).HasMaxLength(2048);
            entity.HasIndex(x => x.EmployerUserId).IsUnique();
            entity.HasOne(x => x.EmployerUser).WithOne(x => x.Company)
                .HasForeignKey<N5Company>(x => x.EmployerUserId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<N5Skill>(entity =>
        {
            entity.ToTable("N5Skills", table => table.HasCheckConstraint(
                "CK_N5Skills_Name", "LEN(LTRIM(RTRIM([Name]))) > 0 AND LEN(LTRIM(RTRIM([NormalizedName]))) > 0"));
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.NormalizedName).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.NormalizedName).IsUnique();
        });

        builder.Entity<N5CandidateSkill>(entity =>
        {
            entity.ToTable("N5CandidateSkills");
            entity.HasKey(x => new { x.CandidateProfileId, x.SkillId });
            entity.HasOne(x => x.CandidateProfile).WithMany(x => x.Skills)
                .HasForeignKey(x => x.CandidateProfileId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Skill).WithMany(x => x.CandidateSkills)
                .HasForeignKey(x => x.SkillId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<N5JobPosting>(entity =>
        {
            entity.ToTable("N5JobPostings", table =>
            {
                table.HasCheckConstraint("CK_N5JobPostings_Status", "[Status] IN (0, 1, 2)");
                table.HasCheckConstraint("CK_N5JobPostings_Utc", "DATEPART(TZOFFSET, [Deadline]) = 0 AND ([PublishedAt] IS NULL OR DATEPART(TZOFFSET, [PublishedAt]) = 0)");
                table.HasCheckConstraint("CK_N5JobPostings_PublishedAt", "[Status] = 0 OR [PublishedAt] IS NOT NULL");
            });
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(10000).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(200);
            entity.Property(x => x.SalaryText).HasMaxLength(200);
            entity.Property(x => x.PublishedAt).HasConversion(
                value => value.HasValue ? value.Value.ToUniversalTime() : (DateTimeOffset?)null,
                value => value.HasValue ? value.Value.ToUniversalTime() : (DateTimeOffset?)null);
            entity.Property(x => x.Deadline).HasConversion(value => value.ToUniversalTime(), value => value.ToUniversalTime());
            entity.Property(x => x.Status).HasConversion<int>();
            entity.HasIndex(x => new { x.Status, x.Deadline, x.PublishedAt });
            entity.HasIndex(x => new { x.CompanyId, x.Status });
            entity.HasOne(x => x.Company).WithMany(x => x.JobPostings)
                .HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<N5JobSkill>(entity =>
        {
            entity.ToTable("N5JobSkills");
            entity.HasKey(x => new { x.JobPostingId, x.SkillId });
            entity.HasOne(x => x.JobPosting).WithMany(x => x.Skills)
                .HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Skill).WithMany(x => x.JobSkills)
                .HasForeignKey(x => x.SkillId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<N5Application>(entity =>
        {
            entity.ToTable("N5Applications", table =>
            {
                table.HasCheckConstraint("CK_N5Applications_Status", "[Status] IN (0, 1, 2, 3)");
                table.HasCheckConstraint("CK_N5Applications_Utc", "DATEPART(TZOFFSET, [AppliedAt]) = 0");
            });
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CoverLetter).HasMaxLength(2000);
            entity.Property(x => x.AppliedAt).HasConversion(value => value.ToUniversalTime(), value => value.ToUniversalTime());
            entity.Property(x => x.Status).HasConversion<int>();
            entity.HasIndex(x => new { x.JobPostingId, x.CandidateProfileId }).IsUnique();
            entity.HasIndex(x => new { x.JobPostingId, x.Status });
            entity.HasOne(x => x.JobPosting).WithMany(x => x.Applications)
                .HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.CandidateProfile).WithMany(x => x.Applications)
                .HasForeignKey(x => x.CandidateProfileId).OnDelete(DeleteBehavior.NoAction);
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        NormalizeSkills();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        NormalizeSkills();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void NormalizeSkills()
    {
        foreach (var entry in ChangeTracker.Entries<N5Skill>()
                     .Where(x => x.State is EntityState.Added or EntityState.Modified))
        {
            if (string.IsNullOrWhiteSpace(entry.Entity.Name))
                throw new InvalidOperationException("Tên kỹ năng không được để trống.");
            entry.Entity.Name = string.Join(" ", entry.Entity.Name.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            entry.Entity.NormalizedName = N5Skill.NormalizeName(entry.Entity.Name);
        }
    }
}
