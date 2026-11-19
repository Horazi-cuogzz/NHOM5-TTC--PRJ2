using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitHub.Web.Data;
using RecruitHub.Web.Models;
using RecruitHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => "Giá trị nhập chưa hợp lệ.");
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => "Vui lòng nhập thông tin bắt buộc.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(field => "Vui lòng nhập một số hợp lệ.");
});
builder.Services.AddDbContext<N5RecruitHubContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<N5AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddEntityFrameworkStores<N5RecruitHubContext>().AddDefaultTokenProviders()
  .AddErrorDescriber<N5IdentityErrorDescriber>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/tai-khoan/dang-nhap";
    options.AccessDeniedPath = "/tai-khoan/khong-co-quyen";
    options.Cookie.Name = "N5.RecruitHub";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Events.OnRedirectToLogin = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api")) ctx.Response.StatusCode = 401;
        else ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api")) ctx.Response.StatusCode = 403;
        else ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };
});
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<N5JobService>();
builder.Services.AddScoped<N5MatchingService>();
builder.Services.AddScoped<N5RecruitmentService>();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/loi");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStatusCodePagesWithReExecute("/trang-loi/{0}");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute("default", "{controller=N5Home}/{action=Index}/{id?}");
if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Database:Initialize"))
    await N5DbSeeder.SeedAsync(app.Services, app.Configuration);
app.Run();

