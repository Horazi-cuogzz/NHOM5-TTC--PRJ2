using System.ComponentModel.DataAnnotations;
using RecruitHub.Web.Models;
namespace RecruitHub.Web.ViewModels;
public record N5SkillOption(int Id, string Name, bool IsActive = true);
public class N5JobCardViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string? Location { get; set; }
    public string? SalaryText { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public List<N5SkillOption> RequiredSkills { get; set; } = [];
    public List<N5SkillOption> MatchedSkills { get; set; } = [];
    public List<N5SkillOption> MissingSkills { get; set; } = [];
    public decimal? ScorePercent { get; set; }
    public int MatchedCount => MatchedSkills.Count;
    public int RequiredCount => RequiredSkills.Count;
}
public class N5JobListViewModel
{
    public string? Query { get; set; }
    public string? Location { get; set; }
    public int? SkillId { get; set; }
    public int Page { get; set; } = 1;
    public int Total { get; set; }
    public int PageSize { get; set; } = 6;
    public int Pages => Math.Max(1, (int)Math.Ceiling((double)Total / PageSize));
    public List<N5JobCardViewModel> Jobs { get; set; } = [];
    public List<N5SkillOption> Skills { get; set; } = [];
    public List<string> Locations { get; set; } = [];
    public List<N5SkillOption> MySkills { get; set; } = [];
    public bool IsRecommendation { get; set; }
}
public class N5JobDetailsViewModel
{
    public N5JobPosting Job { get; set; } = null!;
    public N5JobCardViewModel Card { get; set; } = null!;
    public int? ExistingApplicationId { get; set; }
    public bool CanApply { get; set; }
    [StringLength(2000, ErrorMessage = "Thư giới thiệu tối đa 2.000 ký tự.")]
    public string? CoverLetter { get; set; }
}
public class N5JobEditViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề."), StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự.")]
    public string Title { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập mô tả."), StringLength(10000, ErrorMessage = "Mô tả tối đa 10.000 ký tự.")]
    public string Description { get; set; } = "";
    [StringLength(200, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string? Location { get; set; }
    [StringLength(200, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string? SalaryText { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn hạn nhận hồ sơ."), DataType(DataType.Date)]
    public DateTime Deadline { get; set; } = DateTime.Today.AddDays(30);
    public int[] SkillIds { get; set; } = [];
    public List<N5SkillOption> SkillOptions { get; set; } = [];
    public bool SkillsLocked { get; set; }
    public N5JobStatus Status { get; set; }
}

