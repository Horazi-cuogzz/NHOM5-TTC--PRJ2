using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.Services;
namespace RecruitHub.Web.Controllers;
[ApiController]
public class N5ApiController(N5RecruitHubContext db, N5MatchingService matching, TimeProvider clock) : ControllerBase
{
    [HttpGet("/api/recommendations/me"), Authorize(Roles = "Candidate")]
    public async Task<IActionResult> Recommendations(int limit = 10)
    {
        if (limit is < 1 or > 50) return BadRequest(new { message = "Giới hạn phải từ 1 đến 50." });
        var rows = await matching.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, limit);
        return Ok(rows.Select(j => new
        {
            jobId = j.Id, title = j.Title, companyName = j.CompanyName, matchedCount = j.MatchedCount, requiredCount = j.RequiredCount,
            scorePercent = Math.Round(j.ScorePercent ?? 0, 2), matchedSkills = j.MatchedSkills.Select(s => s.Name), missingSkills = j.MissingSkills.Select(s => s.Name)
        }));
    }
    [HttpGet("/api/employer/summary"), Authorize(Roles = "Employer")]
    public async Task<IActionResult> Summary()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var company = await db.Companies.AsNoTracking().SingleOrDefaultAsync(c => c.EmployerUserId == userId);
        if (company == null) return Ok(new { companyName = "", hasCompany = false, openJobs = 0, totalApplications = 0, byStatus = new Dictionary<string, int>() });
        var now = clock.GetUtcNow();
        var counts = await db.Applications.Where(a => a.JobPosting.CompanyId == company.Id).GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync();
        return Ok(new
        {
            companyName = company.Name, hasCompany = true,
            openJobs = await db.JobPostings.CountAsync(j => j.CompanyId == company.Id && j.Status == N5JobStatus.Published && j.Deadline >= now),
            totalApplications = counts.Sum(s => s.Count),
            byStatus = Enum.GetValues<N5ApplicationStatus>().ToDictionary(s => s.ToString(), s => counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0)
        });
    }
}

