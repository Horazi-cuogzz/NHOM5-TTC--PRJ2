namespace RecruitHub.Web.Models;

public class N5CandidateProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Location { get; set; }
    public string Introduction { get; set; } = string.Empty;
    public string? Education { get; set; }
    public string? Experience { get; set; }
    public N5AppUser User { get; set; } = null!;
    public ICollection<N5CandidateSkill> Skills { get; set; } = new List<N5CandidateSkill>();
    public ICollection<N5Application> Applications { get; set; } = new List<N5Application>();
}
