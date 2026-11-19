using System.ComponentModel.DataAnnotations;
namespace RecruitHub.Web.ViewModels;
public class N5ProfileViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên."), StringLength(100, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại."), StringLength(30, ErrorMessage = "Nội dung tối đa {1} ký tự."), Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string Phone { get; set; } = "";
    [StringLength(200, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string? Location { get; set; }
    [Required(ErrorMessage = "Vui lòng viết giới thiệu ngắn."), StringLength(2000, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string Introduction { get; set; } = "";
    [StringLength(4000, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string? Education { get; set; }
    [StringLength(4000, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string? Experience { get; set; }
    public int[] SkillIds { get; set; } = [];
    public List<N5SkillOption> SkillOptions { get; set; } = [];
}
public class N5CompanyViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên công ty."), StringLength(200, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string Name { get; set; } = "";
    [StringLength(4000, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string? Description { get; set; }
    [StringLength(500, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string? Address { get; set; }
    [StringLength(2048, ErrorMessage = "Nội dung tối đa {1} ký tự."), Url(ErrorMessage = "Website cần có dạng https://example.com.")] public string? Website { get; set; }
}
public class N5SkillEditViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập tên kỹ năng."), StringLength(100, ErrorMessage = "Nội dung tối đa {1} ký tự.")] public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

