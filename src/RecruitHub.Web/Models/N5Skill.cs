namespace RecruitHub.Web.Models;

public class N5Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<N5CandidateSkill> CandidateSkills { get; set; } = new List<N5CandidateSkill>();
    public ICollection<N5JobSkill> JobSkills { get; set; } = new List<N5JobSkill>();

    public static string NormalizeName(string name) =>
        string.Join(" ", name.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
}
