using System.Data;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
namespace RecruitHub.Web.Services;

public class N5RecruitmentService(N5RecruitHubContext db, TimeProvider clock)
{
    public async Task<(int? Id, string? Error)> ApplyAsync(string userId, int jobId, string? letter)
    {
        var trimmedLetter = string.IsNullOrWhiteSpace(letter) ? null : letter.Trim();
        if (trimmedLetter?.Length > 2000) return (null, "Thư giới thiệu tối đa 2.000 ký tự.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var profile = await db.CandidateProfiles.Include(p => p.User).Include(p => p.Skills)
            .SingleOrDefaultAsync(p => p.UserId == userId);
        if (profile == null || string.IsNullOrWhiteSpace(profile.User.FullName) || string.IsNullOrWhiteSpace(profile.Phone)
            || string.IsNullOrWhiteSpace(profile.Introduction) || profile.Skills.Count == 0)
            return (null, "Vui lòng hoàn thiện họ tên, số điện thoại, giới thiệu và ít nhất một kỹ năng trong hồ sơ.");
        var existing = await db.Applications.SingleOrDefaultAsync(a => a.JobPostingId == jobId && a.CandidateProfileId == profile.Id);
        if (existing != null) return (existing.Id, null);
        var job = await db.JobPostings.SingleOrDefaultAsync(j => j.Id == jobId);
        if (job == null || job.Status != N5JobStatus.Published || job.Deadline < clock.GetUtcNow())
            return (null, "Tin đã đóng hoặc hết hạn nhận hồ sơ.");
        var application = new N5Application
        {
            JobPostingId = jobId, CandidateProfileId = profile.Id, CoverLetter = trimmedLetter,
            AppliedAt = clock.GetUtcNow(), Status = N5ApplicationStatus.Submitted
        };
        db.Applications.Add(application);
        try
        {
            await db.SaveChangesAsync(); await tx.CommitAsync();
            return (application.Id, null);
        }
        catch (DbUpdateException ex) when (N5Display.IsDuplicate(ex))
        {
            return (null, "Bạn đã gửi đơn vào tin này. Hãy xem mục Đơn đã gửi.");
        }
    }

    public async Task<string?> ChangeStatusAsync(string employerId, int id, N5ApplicationStatus target)
    {
        N5ApplicationStatus previous;
        if (target == N5ApplicationStatus.Reviewing) previous = N5ApplicationStatus.Submitted;
        else if (target is N5ApplicationStatus.Accepted or N5ApplicationStatus.Rejected) previous = N5ApplicationStatus.Reviewing;
        else return "Trạng thái chuyển tiếp không hợp lệ.";
        // Điều kiện trạng thái nằm trong UPDATE để tránh ghi đè khi hai cửa sổ cùng xử lý.
        var changed = await db.Applications.Where(a => a.Id == id &&
            a.JobPosting.Company.EmployerUserId == employerId && a.Status == previous)
            .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.Status, target));
        return changed == 1 ? null : "Không thể chuyển trạng thái. Đơn không thuộc công ty hoặc đã được xử lý.";
    }
}

