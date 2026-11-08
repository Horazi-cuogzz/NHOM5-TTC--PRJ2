using System.ComponentModel.DataAnnotations;
namespace RecruitHub.Web.ViewModels;
public class N5LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email."), EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu."), DataType(DataType.Password)]
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
public class N5RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên."), StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    public string FullName { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập email."), EmailAddress(ErrorMessage = "Email không hợp lệ."), StringLength(256, ErrorMessage = "Nội dung tối đa {1} ký tự.")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu."), MinLength(8, ErrorMessage = "Mật khẩu cần ít nhất 8 ký tự."), DataType(DataType.Password)]
    public string Password { get; set; } = "";
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại chưa khớp."), DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
    public string Role { get; set; } = "Candidate";
}

