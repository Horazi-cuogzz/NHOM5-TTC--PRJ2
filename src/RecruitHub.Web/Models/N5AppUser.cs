using Microsoft.AspNetCore.Identity;

namespace RecruitHub.Web.Models;

public class N5AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public N5CandidateProfile? CandidateProfile { get; set; }
    public N5Company? Company { get; set; }
}
