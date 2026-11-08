using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Services;

public class N5MatchingService(N5RecruitHubContext db, N5JobService jobs, TimeProvider clock)
{
    public async Task<List<N5JobCardViewModel>> GetAsync(string userId, int limit = 50)
    {
        var mySkills = (await jobs.MySkillsAsync(userId)).Select(s => s.Id).ToHashSet();
        if (mySkills.Count == 0) return [];
        var now = clock.GetUtcNow();
        var candidates = await db.JobPostings.AsNoTracking()
            .Where(j => j.Status == N5JobStatus.Published && j.Deadline >= now &&
                j.Skills.Any(s => mySkills.Contains(s.SkillId)) &&
                !j.Applications.Any(a => a.CandidateProfile.UserId == userId))
            .Include(j => j.Company).Include(j => j.Skills).ThenInclude(s => s.Skill)
            .AsSplitQuery().ToListAsync();
        // Xếp theo điểm gốc; chỉ làm tròn tại giao diện hoặc DTO.
        return candidates.Select(j => N5JobService.Card(j, mySkills))
            .Where(j => j.RequiredCount > 0 && j.ScorePercent > 0)
            .OrderByDescending(j => j.ScorePercent).ThenByDescending(j => j.PublishedAt).ThenBy(j => j.Id)
            .Take(Math.Clamp(limit, 1, 50)).ToList();
    }
}

