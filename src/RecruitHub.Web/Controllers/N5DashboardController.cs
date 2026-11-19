using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace RecruitHub.Web.Controllers;
[Authorize(Roles = "Employer")]
public class N5DashboardController : Controller
{
    [HttpGet("nha-tuyen-dung/tong-quan")]
    public IActionResult Index() => View("N5Index");
}

