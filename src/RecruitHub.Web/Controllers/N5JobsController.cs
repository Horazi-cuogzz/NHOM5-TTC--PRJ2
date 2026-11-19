using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.Services;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Controllers;

[Route("viec-lam")]
public class N5JobsController(N5RecruitHubContext db, N5JobService jobs, N5MatchingService matching, N5RecruitmentService recruitment, TimeProvider clock) : Controller
{
    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    [HttpGet("")]
    public async Task<IActionResult> Index(string? query, string? location, int? skillId, int page = 1) =>
        View("N5Index", await jobs.FindAsync(query, location, skillId, page, User.IsInRole("Candidate") ? UserId : null));
    [HttpGet("phu-hop"), Authorize(Roles = "Candidate")]
    public async Task<IActionResult> Recommended() => View("N5Recommended", new N5JobListViewModel
    {
        Jobs = await matching.GetAsync(UserId!), MySkills = await jobs.MySkillsAsync(UserId), IsRecommendation = true
    });
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var job = await db.JobPostings.AsNoTracking().Include(j => j.Company).Include(j => j.Skills).ThenInclude(s => s.Skill)
            .AsSplitQuery().SingleOrDefaultAsync(j => j.Id == id);
        if (job == null) return NotFound();
        var existing = User.IsInRole("Candidate") ? await db.Applications
            .Where(a => a.JobPostingId == id && a.CandidateProfile.UserId == UserId).Select(a => (int?)a.Id).SingleOrDefaultAsync() : null;
        var open = job.Status == N5JobStatus.Published && job.Deadline >= clock.GetUtcNow();
        if (!open && existing == null && !(User.IsInRole("Employer") && job.Company.EmployerUserId == UserId)) return NotFound();
        var skills = User.IsInRole("Candidate") ? (await jobs.MySkillsAsync(UserId)).Select(s => s.Id).ToHashSet() : null;
        return View("N5Details", new N5JobDetailsViewModel
        {
            Job = job, Card = N5JobService.Card(job, skills), ExistingApplicationId = existing,
            CanApply = open && User.IsInRole("Candidate") && existing == null
        });
    }
    [HttpPost("{id:int}/ung-tuyen"), Authorize(Roles = "Candidate")]
    public async Task<IActionResult> Apply(int id, string? coverLetter)
    {
        var result = await recruitment.ApplyAsync(UserId!, id, coverLetter);
        if (result.Error != null)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["Success"] = "Đơn ứng tuyển đã được ghi nhận.";
        return RedirectToAction("Details", "N5Applications", new { id = result.Id });
    }
}

