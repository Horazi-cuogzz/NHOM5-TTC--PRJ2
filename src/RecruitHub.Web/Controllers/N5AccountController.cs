using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.ViewModels;
namespace RecruitHub.Web.Controllers;

[Route("tai-khoan")]
public class N5AccountController(UserManager<N5AppUser> users, SignInManager<N5AppUser> signIn, N5RecruitHubContext db) : Controller
{
    [HttpGet("dang-nhap"), AllowAnonymous]
    public IActionResult Login(string? returnUrl) => View("N5Login", new N5LoginViewModel { ReturnUrl = returnUrl });
    [HttpPost("dang-nhap"), AllowAnonymous]
    public async Task<IActionResult> Login(N5LoginViewModel model)
    {
        if (!ModelState.IsValid) return View("N5Login", model);
        var result = await signIn.PasswordSignInAsync(model.Email.Trim(), model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded) return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl!) : RedirectToAction("Index", "N5Home");
        ModelState.AddModelError("", result.IsLockedOut ? "Tài khoản tạm khóa do nhập sai nhiều lần. Vui lòng thử lại sau 5 phút." : "Email hoặc mật khẩu chưa đúng.");
        return View("N5Login", model);
    }
    [HttpGet("dang-ky"), AllowAnonymous]
    public IActionResult Register() => View("N5Register", new N5RegisterViewModel());
    [HttpPost("dang-ky"), AllowAnonymous]
    public async Task<IActionResult> Register(N5RegisterViewModel model)
    {
        if (model.Role is not ("Candidate" or "Employer")) ModelState.AddModelError(nameof(model.Role), "Vui lòng chọn ứng viên hoặc nhà tuyển dụng.");
        if (!ModelState.IsValid) return View("N5Register", model);
        await using var tx = await db.Database.BeginTransactionAsync();
        var user = new N5AppUser { UserName = model.Email.Trim(), Email = model.Email.Trim(), FullName = model.FullName.Trim() };
        var created = await users.CreateAsync(user, model.Password);
        if (created.Succeeded) created = await users.AddToRoleAsync(user, model.Role);
        if (!created.Succeeded)
        {
            foreach (var error in created.Errors) ModelState.AddModelError("", error.Description);
            return View("N5Register", model);
        }
        await tx.CommitAsync();
        await signIn.SignInAsync(user, false);
        TempData["Success"] = "Tạo tài khoản thành công. Hãy bổ sung thông tin để bắt đầu.";
        return model.Role == "Candidate" ? RedirectToAction("Edit", "N5Profile") : RedirectToAction("Edit", "N5Company");
    }
    [HttpPost("dang-xuat"), Authorize]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction("Index", "N5Home");
    }
    [HttpGet("khong-co-quyen")]
    public IActionResult Denied()
    {
        Response.StatusCode = 403;
        return View("N5Denied");
    }
}

