using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.Services;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Controllers;

[Authorize(Roles = "Employer"), Route("nha-tuyen-dung/tin")]
public class N5EmployerJobsController(N5RecruitHubContext db, N5JobService jobs) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var query = db.JobPostings.AsNoTracking().Where(j => j.Company.EmployerUserId == UserId);
        search = search?.Trim();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(j => j.Title.Contains(search));
        ViewBag.Search = search;
        var total = await query.CountAsync();
        page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(total / 12d)));
        ViewBag.Page = page; ViewBag.Pages = Math.Max(1, (int)Math.Ceiling(total / 12d)); ViewBag.Total = total;
        return View("N5Index", await query.Include(j => j.Applications).OrderByDescending(j => j.Id).Skip((page - 1) * 12).Take(12).ToListAsync());
    }
    [HttpGet("tao")]
    public async Task<IActionResult> Create()
    {
        if (!await db.Companies.AnyAsync(c => c.EmployerUserId == UserId))
        { TempData["Error"] = "Hãy bổ sung thông tin công ty trước khi tạo tin."; return RedirectToAction("Edit", "N5Company"); }
        return View("N5Edit", new N5JobEditViewModel { SkillOptions = await jobs.SkillOptionsAsync() });
    }
    [HttpGet("{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var j = await db.JobPostings.Include(j => j.Skills).SingleOrDefaultAsync(j => j.Id == id && j.Company.EmployerUserId == UserId);
        if (j == null) return NotFound();
        if (j.Status == N5JobStatus.Closed) { TempData["Error"] = "Tin đã đóng không thể sửa."; return RedirectToAction(nameof(Index)); }
        var ids = j.Skills.Select(s => s.SkillId).ToArray();
        return View("N5Edit", new N5JobEditViewModel
        {
            Id = j.Id, Title = j.Title, Description = j.Description, Location = j.Location, SalaryText = j.SalaryText,
            Deadline = j.Deadline.ToOffset(TimeSpan.FromHours(7)).Date, Status = j.Status, SkillIds = ids,
            SkillOptions = await jobs.SkillOptionsAsync(ids), SkillsLocked = await db.Applications.AnyAsync(a => a.JobPostingId == id)
        });
    }
    [HttpPost("luu")]
    public async Task<IActionResult> Save(N5JobEditViewModel model)
    {
        var j = model.Id == 0 ? null : await db.JobPostings.AsNoTracking().Include(j => j.Skills)
            .SingleOrDefaultAsync(j => j.Id == model.Id && j.Company.EmployerUserId == UserId);
        if (model.Id != 0 && j == null) return NotFound();
        model.Status = j?.Status ?? N5JobStatus.Draft;
        model.SkillsLocked = model.Id != 0 && await db.Applications.AnyAsync(a => a.JobPostingId == model.Id);
        model.SkillOptions = await jobs.SkillOptionsAsync(j?.Skills.Select(s => s.SkillId).ToArray());
        if (!ModelState.IsValid) return View("N5Edit", model);
        var result = await jobs.SaveAsync(UserId, model);
        if (result.Error != null) { ModelState.AddModelError("", result.Error); return View("N5Edit", model); }
        TempData["Success"] = "Đã lưu tin tuyển dụng.";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost("{id:int}/dang")]
    public async Task<IActionResult> Publish(int id)
    {
        var error = await jobs.PublishAsync(id, UserId);
        TempData[error == null ? "Success" : "Error"] = error ?? "Tin đã được đăng công khai.";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost("{id:int}/dong")]
    public async Task<IActionResult> Close(int id)
    {
        var changed = await db.JobPostings.Where(j => j.Id == id && j.Company.EmployerUserId == UserId && j.Status == N5JobStatus.Published)
            .ExecuteUpdateAsync(s => s.SetProperty(j => j.Status, N5JobStatus.Closed));
        TempData[changed == 1 ? "Success" : "Error"] = changed == 1 ? "Đã đóng tin. Các đơn cũ vẫn được giữ để xử lý." : "Chỉ có thể đóng tin đã đăng thuộc công ty của bạn.";
        return RedirectToAction(nameof(Index));
    }
}

