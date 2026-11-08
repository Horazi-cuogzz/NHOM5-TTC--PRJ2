using RecruitHub.Web.Models;
namespace RecruitHub.Web.ViewModels;
public class N5ApplicationListViewModel
{
    public List<N5Application> Applications { get; set; } = [];
    public List<N5JobPosting> Jobs { get; set; } = [];
    public int? JobId { get; set; }
    public N5ApplicationStatus? Status { get; set; }
    public bool IsEmployer { get; set; }
    public int Page { get; set; } = 1;
    public int Total { get; set; }
    public int Pages => Math.Max(1, (int)Math.Ceiling(Total / 12d));
}

