using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Controllers;

[Authorize(Roles = "Employer"), Route("cong-ty")]
public class N5CompanyController(N5RecruitHubContext db) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet("")]
    public async Task<IActionResult> Edit()
    {
        var c = await db.Companies.SingleOrDefaultAsync(c => c.EmployerUserId == UserId);
        return View("N5Edit", c == null ? new N5CompanyViewModel() : new N5CompanyViewModel
            { Name = c.Name, Description = c.Description, Address = c.Address, Website = c.Website });
    }
    [HttpPost("")]
    public async Task<IActionResult> Edit(N5CompanyViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Website) &&
            (!Uri.TryCreate(model.Website, UriKind.Absolute, out var url) || url.Scheme is not ("https" or "http")))
            ModelState.AddModelError(nameof(model.Website), "Website phải bắt đầu bằng http:// hoặc https://.");
        if (!ModelState.IsValid) return View("N5Edit", model);
        var c = await db.Companies.SingleOrDefaultAsync(c => c.EmployerUserId == UserId);
        if (c == null) { c = new N5Company { EmployerUserId = UserId }; db.Companies.Add(c); }
        c.Name = model.Name.Trim(); c.Description = model.Description?.Trim(); c.Address = model.Address?.Trim(); c.Website = model.Website?.Trim();
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã lưu thông tin công ty.";
        return RedirectToAction(nameof(Edit));
    }
}

