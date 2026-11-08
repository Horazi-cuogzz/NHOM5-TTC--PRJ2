namespace RecruitHub.Web.Models;

public class N5JobSkill
{
    public int JobPostingId { get; set; }
    public int SkillId { get; set; }
    public N5JobPosting JobPosting { get; set; } = null!;
    public N5Skill Skill { get; set; } = null!;
}
