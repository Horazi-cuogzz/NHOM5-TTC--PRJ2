namespace RecruitHub.Web.Models;

public class N5CandidateSkill
{
    public int CandidateProfileId { get; set; }
    public int SkillId { get; set; }
    public N5CandidateProfile CandidateProfile { get; set; } = null!;
    public N5Skill Skill { get; set; } = null!;
}
