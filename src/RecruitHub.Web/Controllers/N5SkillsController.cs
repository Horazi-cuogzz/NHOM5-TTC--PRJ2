using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.Services;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Controllers;

[Authorize(Roles = "Admin"), Route("quan-tri/ky-nang")]
public class N5SkillsController(N5RecruitHubContext db) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? query, int page = 1)
    {
        var skills = db.Skills.AsNoTracking();
        query = query?.Trim();
        if (!string.IsNullOrEmpty(query))
            skills = skills.Where(s => s.Name.Contains(query));
        const int pageSize = 12;
        var pages = Math.Max(1, (int)Math.Ceiling(await skills.CountAsync() / (double)pageSize));
        page = Math.Clamp(page, 1, pages);
        ViewBag.Query = query;
        ViewBag.Page = page;
        ViewBag.Pages = pages;
        var rows = await skills.OrderBy(s => s.Name).ThenBy(s => s.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View("N5Index", rows);
    }
    [HttpGet("sua/{id:int?}")]
    public async Task<IActionResult> Edit(int? id)
    {
        var skill = id.HasValue ? await db.Skills.FindAsync(id) : null;
        if (id.HasValue && skill == null) return NotFound();
        return View("N5Edit", skill == null ? new N5SkillEditViewModel() : new N5SkillEditViewModel { Id = skill.Id, Name = skill.Name, IsActive = skill.IsActive });
    }
    [HttpPost("luu")]
    public async Task<IActionResult> Save(N5SkillEditViewModel model)
    {
        if (!ModelState.IsValid) return View("N5Edit", model);
        var normalized = N5Skill.NormalizeName(model.Name);
        if (await db.Skills.AnyAsync(s => s.NormalizedName == normalized && s.Id != model.Id))
        { ModelState.AddModelError(nameof(model.Name), "Tên kỹ năng đã tồn tại."); return View("N5Edit", model); }
        var skill = model.Id == 0 ? new N5Skill() : await db.Skills.FindAsync(model.Id);
        if (skill == null) return NotFound();
        skill.Name = model.Name.Trim(); skill.IsActive = model.IsActive;
        if (model.Id == 0) db.Skills.Add(skill);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (N5Display.IsDuplicate(ex))
        { ModelState.AddModelError(nameof(model.Name), "Tên kỹ năng đã tồn tại."); return View("N5Edit", model); }
        TempData["Success"] = "Đã cập nhật danh mục kỹ năng.";
        return RedirectToAction(nameof(Index));
    }
}

