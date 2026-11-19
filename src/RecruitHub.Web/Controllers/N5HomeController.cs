using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RecruitHub.Web.Services;
namespace RecruitHub.Web.Controllers;
public class N5HomeController(N5JobService jobs, N5MatchingService matching) : Controller
{
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var userId = User.IsInRole("Candidate") ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var model = await jobs.FindAsync(null, null, null, 1, userId, 3);
        if (userId != null)
        {
            model.Jobs = await matching.GetAsync(userId, 3);
            model.IsRecommendation = true;
        }
        return View("N5Index", model);
    }
    [Route("/loi"), Route("/trang-loi/{code:int}")]
    public IActionResult Error(int code = 500)
    {
        if (Request.Path.StartsWithSegments("/api")) return StatusCode(code);
        Response.StatusCode = code;
        ViewBag.StatusCode = code;
        return View("N5Error");
    }
}

