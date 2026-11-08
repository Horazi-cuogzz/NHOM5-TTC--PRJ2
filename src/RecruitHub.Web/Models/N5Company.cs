namespace RecruitHub.Web.Models;

public class N5Company
{
    public int Id { get; set; }
    public string EmployerUserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public N5AppUser EmployerUser { get; set; } = null!;
    public ICollection<N5JobPosting> JobPostings { get; set; } = new List<N5JobPosting>();
}
