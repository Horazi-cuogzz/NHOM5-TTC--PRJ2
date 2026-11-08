namespace RecruitHub.Web.Models;

public class N5Application
{
    public int Id { get; set; }
    public int JobPostingId { get; set; }
    public int CandidateProfileId { get; set; }
    public string? CoverLetter { get; set; }
    public DateTimeOffset AppliedAt { get; set; } = DateTimeOffset.UtcNow;
    public N5ApplicationStatus Status { get; set; } = N5ApplicationStatus.Submitted;
    public N5JobPosting JobPosting { get; set; } = null!;
    public N5CandidateProfile CandidateProfile { get; set; } = null!;
}
