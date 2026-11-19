using System.Data;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Services;

public class N5JobService(N5RecruitHubContext db, TimeProvider clock)
{
    public Task<List<N5SkillOption>> MySkillsAsync(string? userId) =>
        db.CandidateSkills.AsNoTracking().Where(s => s.CandidateProfile.UserId == userId)
          .OrderBy(s => s.Skill.Name).Select(s => new N5SkillOption(s.SkillId, s.Skill.Name, s.Skill.IsActive)).ToListAsync();

    public Task<List<N5SkillOption>> SkillOptionsAsync(int[]? current = null)
    {
        current ??= [];
        return db.Skills.AsNoTracking().Where(s => s.IsActive || current.Contains(s.Id))
            .OrderBy(s => s.Name).Select(s => new N5SkillOption(s.Id, s.Name, s.IsActive)).ToListAsync();
    }

    public async Task<N5JobListViewModel> FindAsync(string? query, string? location, int? skillId, int page, string? userId, int size = 6)
    {
        var now = clock.GetUtcNow();
        var jobs = db.JobPostings.AsNoTracking().Where(j => j.Status == N5JobStatus.Published && j.Deadline >= now);
        query = query?.Trim();
        if (!string.IsNullOrWhiteSpace(query))
            jobs = jobs.Where(j => j.Title.Contains(query) || j.Company.Name.Contains(query) ||
                                  j.Skills.Any(s => s.Skill.Name.Contains(query)));
        if (!string.IsNullOrWhiteSpace(location)) jobs = jobs.Where(j => j.Location == location);
        if (skillId.HasValue) jobs = jobs.Where(j => j.Skills.Any(s => s.SkillId == skillId));
        
        var mySkills = await MySkillsAsync(userId);
        var candidateSkills = userId == null ? null : mySkills.Select(s => s.Id).ToHashSet();

        List<N5JobCardViewModel> cards;
        int total;
        if (candidateSkills != null && candidateSkills.Count > 0)
        {
            var all = await jobs.Include(j => j.Company).Include(j => j.Skills).ThenInclude(s => s.Skill)
                .AsSplitQuery().ToListAsync();
            // Sắp xếp: Tin có tỷ lệ phù hợp (% score) cao hơn luôn xếp ở đầu tiên
            var allCards = all.Select(j => Card(j, candidateSkills))
                .OrderByDescending(j => j.ScorePercent ?? -1)
                .ThenByDescending(j => j.PublishedAt)
                .ThenBy(j => j.Id)
                .ToList();
            total = allCards.Count;
            page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling((double)total / size)));
            cards = allCards.Skip((page - 1) * size).Take(size).ToList();
        }
        else
        {
            total = await jobs.CountAsync();
            page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling((double)total / size)));
            var rows = await jobs.Include(j => j.Company).Include(j => j.Skills).ThenInclude(s => s.Skill)
                .OrderByDescending(j => j.PublishedAt).ThenBy(j => j.Id)
                .Skip((page - 1) * size).Take(size).AsSplitQuery().ToListAsync();
            cards = rows.Select(j => Card(j, candidateSkills)).ToList();
        }

        var standardLocations = new[] { "Hà Nội", "TP. Hồ Chí Minh", "Đà Nẵng", "Cần Thơ", "Hải Phòng", "Làm việc từ xa" };
        var dbLocations = await db.JobPostings.AsNoTracking()
            .Where(j => j.Status == N5JobStatus.Published && j.Deadline >= now && j.Location != null && j.Location != "")
            .Select(j => j.Location!).Distinct().ToListAsync();
        var allLocations = standardLocations.Union(dbLocations).Distinct().OrderBy(s => s).ToList();

        return new N5JobListViewModel
        {
            Query = query, Location = location, SkillId = skillId, Page = page, Total = total, PageSize = size,
            Skills = await SkillOptionsAsync(), MySkills = mySkills,
            Locations = allLocations,
            Jobs = cards
        };
    }

    public static N5JobCardViewModel Card(N5JobPosting job, HashSet<int>? candidateSkills)
    {
        var required = job.Skills.Select(s => new N5SkillOption(s.SkillId, s.Skill.Name, s.Skill.IsActive))
            .DistinctBy(s => s.Id).OrderBy(s => s.Name).ToList();
        var matched = required.Where(s => candidateSkills?.Contains(s.Id) == true).ToList();
        return new N5JobCardViewModel
        {
            Id = job.Id, Title = job.Title, CompanyName = job.Company.Name, Location = job.Location,
            SalaryText = job.SalaryText, PublishedAt = job.PublishedAt, Deadline = job.Deadline,
            RequiredSkills = required, MatchedSkills = matched,
            MissingSkills = required.Where(s => candidateSkills?.Contains(s.Id) != true).ToList(),
            ScorePercent = candidateSkills == null || required.Count == 0 ? null : matched.Count * 100m / required.Count
        };
    }

    public async Task<(int Id, string? Error)> SaveAsync(string employerId, N5JobEditViewModel input)
    {
        if (input.Deadline.Year is < 2000 or > 2100)
            return (0, "Hạn nhận hồ sơ phải thuộc giai đoạn 2000–2100.");
        // Cùng mức cô lập với gửi đơn: không thay bộ kỹ năng trong lúc tin nhận đơn đầu tiên.
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var company = await db.Companies.SingleOrDefaultAsync(c => c.EmployerUserId == employerId);
        if (company == null) return (0, "Vui lòng cập nhật công ty trước khi tạo tin.");
        var job = input.Id == 0 ? new N5JobPosting { CompanyId = company.Id, Status = N5JobStatus.Draft }
            : await db.JobPostings.Include(j => j.Skills).SingleOrDefaultAsync(j => j.Id == input.Id && j.CompanyId == company.Id);
        if (job == null) return (0, "Không tìm thấy tin thuộc công ty của bạn.");
        if (job.Status == N5JobStatus.Closed) return (0, "Tin đã đóng không thể sửa.");
        var selected = input.SkillIds.Distinct().ToHashSet();
        var previous = job.Skills.Select(s => s.SkillId).ToHashSet();
        if (input.Id != 0 && await db.Applications.AnyAsync(a => a.JobPostingId == job.Id) && !selected.SetEquals(previous))
            return (0, "Tin đã nhận đơn phải giữ nguyên bộ kỹ năng yêu cầu.");
        var allowed = await db.Skills.Where(s => s.IsActive || previous.Contains(s.Id)).Select(s => s.Id).ToListAsync();
        if (selected.Any(id => !allowed.Contains(id))) return (0, "Có kỹ năng không còn được chọn mới.");
        var deadline = N5Display.EndOfVietnamDay(input.Deadline);
        if (job.Status == N5JobStatus.Published && (selected.Count == 0 || deadline < clock.GetUtcNow()))
            return (0, "Tin đã đăng cần ít nhất một kỹ năng và hạn nhận hồ sơ hợp lệ.");
        job.Title = input.Title.Trim(); job.Description = input.Description.Trim();
        job.Location = input.Location?.Trim(); job.SalaryText = input.SalaryText?.Trim(); job.Deadline = deadline;
        foreach (var removed in job.Skills.Where(s => !selected.Contains(s.SkillId)).ToList())
            db.JobSkills.Remove(removed);
        foreach (var id in selected.Except(previous)) job.Skills.Add(new N5JobSkill { SkillId = id });
        if (input.Id == 0) db.JobPostings.Add(job);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return (job.Id, null);
    }

    public async Task<string?> PublishAsync(int id, string employerId)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var job = await db.JobPostings.Include(j => j.Skills).Include(j => j.Company)
            .SingleOrDefaultAsync(j => j.Id == id && j.Company.EmployerUserId == employerId);
        if (job == null) return "Không tìm thấy tin thuộc công ty của bạn.";
        if (job.Status != N5JobStatus.Draft) return "Chỉ có thể đăng tin từ bản nháp.";
        if (string.IsNullOrWhiteSpace(job.Title) || string.IsNullOrWhiteSpace(job.Description) ||
            string.IsNullOrWhiteSpace(job.Company.Name) || job.Skills.Count == 0 || job.Deadline < clock.GetUtcNow())
            return "Hãy bổ sung tiêu đề, mô tả, công ty, kỹ năng và hạn nhận hồ sơ hợp lệ.";
        job.Status = N5JobStatus.Published; job.PublishedAt = clock.GetUtcNow();
        await db.SaveChangesAsync(); await tx.CommitAsync();
        return null;
    }
}

