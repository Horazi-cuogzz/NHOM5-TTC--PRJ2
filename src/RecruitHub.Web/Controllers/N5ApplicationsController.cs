using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.Services;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Controllers;

[Authorize(Roles = "Candidate,Employer"), Route("don-ung-tuyen")]
public class N5ApplicationsController(N5RecruitHubContext db, N5RecruitmentService recruitment) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet("")]
    public async Task<IActionResult> Index(int? jobId, N5ApplicationStatus? status, int page = 1)
    {
        var employer = User.IsInRole("Employer");
        var query = db.Applications.AsNoTracking().Where(a => employer
            ? a.JobPosting.Company.EmployerUserId == UserId : a.CandidateProfile.UserId == UserId);
        if (jobId.HasValue) query = query.Where(a => a.JobPostingId == jobId);
        if (status.HasValue) query = query.Where(a => a.Status == status);
        var total = await query.CountAsync();
        page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(total / 12d)));
        return View("N5Index", new N5ApplicationListViewModel
        {
            IsEmployer = employer, JobId = jobId, Status = status, Page = page, Total = total,
            Jobs = employer ? await db.JobPostings.AsNoTracking().Where(j => j.Company.EmployerUserId == UserId).OrderByDescending(j => j.Id).ToListAsync() : [],
            Applications = await query.Include(a => a.JobPosting).ThenInclude(j => j.Company)
                .Include(a => a.CandidateProfile).ThenInclude(p => p.User)
                .OrderByDescending(a => a.AppliedAt).ThenBy(a => a.Id).Skip((page - 1) * 12).Take(12).ToListAsync()
        });
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var employer = User.IsInRole("Employer");
        var a = await db.Applications.AsNoTracking()
            .Where(a => a.Id == id && (employer ? a.JobPosting.Company.EmployerUserId == UserId : a.CandidateProfile.UserId == UserId))
            .Include(a => a.JobPosting).ThenInclude(j => j.Company)
            .Include(a => a.CandidateProfile).ThenInclude(p => p.User)
            .Include(a => a.CandidateProfile).ThenInclude(p => p.Skills).ThenInclude(s => s.Skill)
            .AsSplitQuery().SingleOrDefaultAsync();
        return a == null ? NotFound() : View("N5Details", a);
    }
    [HttpPost("{id:int}/trang-thai"), Authorize(Roles = "Employer")]
    public async Task<IActionResult> ChangeStatus(int id, N5ApplicationStatus status)
    {
        var error = await recruitment.ChangeStatusAsync(UserId, id, status);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã cập nhật trạng thái đơn.";
        return RedirectToAction(nameof(Details), new { id });
    }
}

