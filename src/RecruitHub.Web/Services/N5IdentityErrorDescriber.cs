using Microsoft.AspNetCore.Identity;
namespace RecruitHub.Web.Services;
public class N5IdentityErrorDescriber : IdentityErrorDescriber
{
    private static IdentityError Error(string code, string text) => new() { Code = code, Description = text };
    public override IdentityError DuplicateEmail(string email) => Error(nameof(DuplicateEmail), "Email này đã được sử dụng.");
    public override IdentityError DuplicateUserName(string userName) => Error(nameof(DuplicateUserName), "Email này đã được sử dụng.");
    public override IdentityError InvalidEmail(string? email) => Error(nameof(InvalidEmail), "Email không hợp lệ.");
    public override IdentityError PasswordTooShort(int length) => Error(nameof(PasswordTooShort), $"Mật khẩu cần ít nhất {length} ký tự.");
    public override IdentityError PasswordRequiresNonAlphanumeric() => Error(nameof(PasswordRequiresNonAlphanumeric), "Mật khẩu cần có ký tự đặc biệt.");
    public override IdentityError PasswordRequiresDigit() => Error(nameof(PasswordRequiresDigit), "Mật khẩu cần có chữ số.");
    public override IdentityError PasswordRequiresLower() => Error(nameof(PasswordRequiresLower), "Mật khẩu cần có chữ thường.");
    public override IdentityError PasswordRequiresUpper() => Error(nameof(PasswordRequiresUpper), "Mật khẩu cần có chữ hoa.");
    public override IdentityError DefaultError() => Error(nameof(DefaultError), "Thao tác chưa thành công. Vui lòng thử lại.");
}

