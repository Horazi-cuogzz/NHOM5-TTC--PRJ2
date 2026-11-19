using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.Services;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Controllers;

[Authorize(Roles = "Candidate"), Route("ho-so")]
public class N5ProfileController(N5RecruitHubContext db, N5JobService jobs) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet("")]
    public async Task<IActionResult> Edit()
    {
        var user = await db.Users.SingleAsync(u => u.Id == UserId);
        var p = await db.CandidateProfiles.Include(p => p.Skills).SingleOrDefaultAsync(p => p.UserId == UserId);
        var ids = p?.Skills.Select(s => s.SkillId).ToArray() ?? [];
        return View("N5Edit", new N5ProfileViewModel
        {
            FullName = user.FullName, Email = user.Email ?? "", Phone = p?.Phone ?? "", Location = p?.Location,
            Introduction = p?.Introduction ?? "", Education = p?.Education, Experience = p?.Experience,
            SkillIds = ids, SkillOptions = await jobs.SkillOptionsAsync(ids)
        });
    }
    [HttpPost("")]
    public async Task<IActionResult> Edit(N5ProfileViewModel model)
    {
        var user = await db.Users.SingleAsync(u => u.Id == UserId);
        var p = await db.CandidateProfiles.Include(p => p.Skills).SingleOrDefaultAsync(p => p.UserId == UserId);
        var previous = p?.Skills.Select(s => s.SkillId).ToArray() ?? [];
        model.Email = user.Email ?? "";
        model.SkillOptions = await jobs.SkillOptionsAsync(previous);
        var ids = model.SkillIds.Distinct().ToArray();
        if (ids.Length == 0) ModelState.AddModelError(nameof(model.SkillIds), "Vui lòng chọn ít nhất một kỹ năng.");
        if (ids.Any(id => !model.SkillOptions.Any(s => s.Id == id))) ModelState.AddModelError(nameof(model.SkillIds), "Có kỹ năng không còn được chọn mới.");
        if (!ModelState.IsValid) return View("N5Edit", model);
        if (p == null) { p = new N5CandidateProfile { UserId = UserId }; db.CandidateProfiles.Add(p); }
        user.FullName = model.FullName.Trim(); p.Phone = model.Phone.Trim(); p.Location = model.Location?.Trim();
        p.Introduction = model.Introduction.Trim(); p.Education = model.Education?.Trim(); p.Experience = model.Experience?.Trim();
        db.CandidateSkills.RemoveRange(p.Skills.Where(s => !ids.Contains(s.SkillId)).ToList());
        foreach (var id in ids.Except(previous)) p.Skills.Add(new N5CandidateSkill { SkillId = id });
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật hồ sơ và kỹ năng thành công! Dưới đây là các việc làm phù hợp nhất với bạn.";
        return RedirectToAction("Recommended", "N5Jobs");
    }
}

