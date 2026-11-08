namespace RecruitHub.Web.Models;

public class N5JobPosting
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? SalaryText { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public N5JobStatus Status { get; set; } = N5JobStatus.Draft;
    public N5Company Company { get; set; } = null!;
    public ICollection<N5JobSkill> Skills { get; set; } = new List<N5JobSkill>();
    public ICollection<N5Application> Applications { get; set; } = new List<N5Application>();
}
