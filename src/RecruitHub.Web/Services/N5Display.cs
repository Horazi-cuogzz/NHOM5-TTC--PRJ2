using RecruitHub.Web.Models;
namespace RecruitHub.Web.Services;
public static class N5Display
{
    // Mốc thời gian lưu UTC, hạn ngày là hết ngày tại Việt Nam.
    public static DateTimeOffset EndOfVietnamDay(DateTime date) =>
        new DateTimeOffset(date.Date.AddDays(1).AddTicks(-1), TimeSpan.FromHours(7)).ToUniversalTime();
    public static string Date(DateTimeOffset date) => date.ToOffset(TimeSpan.FromHours(7)).ToString("dd/MM/yyyy");
    public static string ApplicationStatus(N5ApplicationStatus status) => status switch
    {
        N5ApplicationStatus.Submitted => "Đã nộp", N5ApplicationStatus.Reviewing => "Đang xem xét",
        N5ApplicationStatus.Accepted => "Đạt yêu cầu", N5ApplicationStatus.Rejected => "Từ chối", _ => "Không xác định"
    };
    public static string JobStatus(N5JobPosting job) => job.Status switch
    {
        N5JobStatus.Draft => "Bản nháp", N5JobStatus.Closed => "Đã đóng",
        _ => job.Deadline < DateTimeOffset.UtcNow ? "Hết hạn" : "Đang tuyển"
    };
    public static bool IsDuplicate(Microsoft.EntityFrameworkCore.DbUpdateException ex) =>
        ex.InnerException is Microsoft.Data.SqlClient.SqlException sql && sql.Number is 2601 or 2627;
}

