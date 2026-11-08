# Hợp đồng dữ liệu RecruitHub N5

Ngày 28/10/2026. Hợp đồng triển khai theo SRS tuần 1; tên `AppUser` trong SRS được đổi thành `N5AppUser` theo yêu cầu tiền tố N5. Chat Database sở hữu Models, Data, Migrations và database; không sửa Program hoặc csproj của ứng dụng.

## Công nghệ và cấu hình

- Project ứng dụng: `src/RecruitHub.Web/N5RecruitHub.Web.csproj`, .NET 10; EF Core SQL Server, Identity EF và Design cùng phiên bản 10.0.0.
- Restore bộ 10.0.0 từng phát hiện NU1902/NU1903 ở dependency `System.Security.Cryptography.Pkcs` 9.0.4. Chat Code đã thêm tham chiếu trực tiếp 10.0.14 vào Web; tooling Database đồng bộ 10.0.14. EF/Identity vẫn là 10.0.0; không tắt NuGet audit.
- Namespace entity/enum: `RecruitHub.Web.Models`; namespace context/seeder: `RecruitHub.Web.Data`.
- Context: `N5RecruitHubContext : IdentityDbContext<N5AppUser>`; constructor nhận `DbContextOptions<N5RecruitHubContext>`.
- Chuỗi kết nối cấu hình: `ConnectionStrings:DefaultConnection`. Local Windows auth: `Server=.\SQLEXPRESS;Database=N5RecruitHub;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`.
- ID nghiệp vụ là `int` tự tăng; ID Identity là `string`. Mốc thời gian là `DateTimeOffset`, lưu UTC; hiển thị/nhập hạn ngày theo giờ Việt Nam do tầng nghiệp vụ xử lý.
- DbSet: `CandidateProfiles`, `Companies`, `Skills`, `CandidateSkills`, `JobPostings`, `JobSkills`, `Applications`; tài khoản dùng `Users` kế thừa Identity.
- Bảng nghiệp vụ có tiền tố N5; bảng Identity giữ tên chuẩn `AspNet*`.

## Entity và navigation properties

| Entity | Thuộc tính scalar | Navigation |
| --- | --- | --- |
| N5AppUser | Thuộc tính Identity; FullName: string bắt buộc, tối đa 100 | CandidateProfile?; Company? |
| N5CandidateProfile | Id; UserId: string bắt buộc; Phone?: 30; Location?: 200; Introduction: string không null, 2000; Education?: 4000; Experience?: 4000 | User; Skills: ICollection<N5CandidateSkill>; Applications: ICollection<N5Application> |
| N5Company | Id; EmployerUserId bắt buộc; Name: string không null, 200; Description?: 4000; Address?: 500; Website?: 2048 | EmployerUser; JobPostings: ICollection<N5JobPosting> |
| N5Skill | Id; Name: string bắt buộc, 100; NormalizedName: string bắt buộc, 100; IsActive: bool mặc định true | CandidateSkills; JobSkills |
| N5CandidateSkill | CandidateProfileId; SkillId | CandidateProfile; Skill |
| N5JobPosting | Id; CompanyId; Title: string không null, 200; Description: string không null, 10000; Location?: 200; SalaryText?: 200; PublishedAt: DateTimeOffset?; Deadline: DateTimeOffset; Status: N5JobStatus | Company; Skills: ICollection<N5JobSkill>; Applications: ICollection<N5Application> |
| N5JobSkill | JobPostingId; SkillId | JobPosting; Skill |
| N5Application | Id; JobPostingId; CandidateProfileId; CoverLetter?: 2000; AppliedAt: DateTimeOffset; Status: N5ApplicationStatus | JobPosting; CandidateProfile |

Dấu `?` nghĩa là cho phép null. Các navigation tới một đối tượng bắt buộc dùng giá trị khởi tạo `null!`; cần Include hoặc projection trước khi đọc. Collection được khởi tạo rỗng. FullName chỉ lưu ở N5AppUser; không lặp lại trong CandidateProfile. Không có CV file, snapshot hồ sơ, bảng điểm matching hoặc lịch sử trạng thái.

## Enum và ràng buộc

- `N5JobStatus`: Draft = 0, Published = 1, Closed = 2.
- `N5ApplicationStatus`: Submitted = 0, Reviewing = 1, Accepted = 2, Rejected = 3.
- Enum lưu số nguyên, có CHECK chỉ nhận giá trị hợp lệ. Hết hạn được suy ra từ Deadline, không thêm enum Expired.
- CHECK yêu cầu PublishedAt có giá trị với tin Published/Closed; kiểm tra offset UTC cho Deadline, PublishedAt, AppliedAt. EF tự chuyển các thời điểm sang UTC khi lưu.
- UNIQUE: CandidateProfile.UserId; Company.EmployerUserId; Skill.NormalizedName; Application(JobPostingId, CandidateProfileId).
- Khóa chính ghép: CandidateSkill(CandidateProfileId, SkillId), JobSkill(JobPostingId, SkillId).
- FK nghiệp vụ dùng NoAction: không xóa dây chuyền hồ sơ, công ty, tin, đơn hoặc các liên kết kỹ năng. Quan hệ tài khoản–hồ sơ và tài khoản–công ty là một–không hoặc một trong quá trình đăng ký, tối đa một bản ghi mỗi loại.
- `N5Skill.NormalizeName(string)` chuẩn hóa khoảng trắng và chuyển hoa invariant, giữ ký hiệu C, C#, C++. DbContext cập nhật NormalizedName từ Name khi thêm/sửa kỹ năng; unique index là hàng rào chống trùng.
- Dữ liệu nháp/hồ sơ chưa hoàn chỉnh có thể chứa chuỗi rỗng. Tầng Service kiểm tra điều kiện đầy đủ trước đăng tin/nộp đơn.

## Phần nghiệp vụ chat Code cần thực hiện

Các ràng buộc liên bảng/phụ thuộc trạng thái không được thay thế bằng FK: đúng vai trò và chủ sở hữu; Published còn hạn mới nhận đơn; hồ sơ có thông tin tối thiểu và kỹ năng; tin có ít nhất một kỹ năng khi đăng; không đổi bộ kỹ năng sau khi có đơn; giữ kỹ năng cũ đã ngừng dùng nhưng không chọn mới; chỉ chuyển trạng thái theo SRS. Không có thao tác xóa tin, mở lại, rút/nộp lại đơn hoặc đảo trạng thái kết thúc.

Gợi ý tính theo tập SkillId, không lưu điểm. Lọc Published còn hạn, chưa nộp và có điểm > 0; xếp điểm chưa làm tròn giảm dần, PublishedAt giảm dần, Id tăng dần. Truy vấn kỹ năng đã liên kết không loại IsActive=false vì BR14.

## API seed và tích hợp Program

`await N5DbSeeder.SeedAsync(IServiceProvider services, IConfiguration configuration);`

Program chỉ gọi khi `app.Environment.IsDevelopment()` và `Database:Initialize=true`. Seeder tự tạo scope, áp dụng Migration, tạo ba role Candidate/Employer/Admin và dữ liệu demo trong một transaction. Không dùng EnsureCreated. Có kiểm tra môi trường Development trong seeder.

- Mật khẩu đọc từ `Seed:DemoPassword` (user-secrets hoặc biến môi trường `Seed__DemoPassword`), không có mật khẩu mặc định trong mã nguồn. Khi tạo tài khoản mới cần mật khẩu hợp lệ theo Identity; không đặt lại mật khẩu tài khoản đã có.
- Email demo: `admin@recruithub.example`, `employer1@recruithub.example`, `employer2@recruithub.example`, `candidate1@recruithub.example` đến `candidate5@recruithub.example`.
- Dữ liệu dự kiến: 3 vai trò, 8 tài khoản, 2 công ty, 5 hồ sơ, 20 kỹ năng, 12 tin, các đơn đủ bốn trạng thái. Toàn bộ thông tin giả lập.
- Seed có khóa ứng dụng SQL Server để tránh chạy đồng thời và mốc hoàn tất `N5RecruitHub:DemoSeed=v1` trong AspNetUserClaims của admin demo. Mốc và dữ liệu cùng commit một transaction. Khi mốc đã tồn tại, chỉ áp dụng migration mới rồi bỏ qua toàn bộ dữ liệu demo: không nhân đôi, không ghi đè chỉnh sửa, không gia hạn tin và không đặt lại mật khẩu. Không tự bổ sung lại bản ghi demo đã bị người dùng xóa sau lần seed đầu.
- Mật khẩu demo đã được sinh ngẫu nhiên và lưu ngoài repo trong user-secrets ID `N5RecruitHub-Local-Development`, dùng chung Web và tooling. Không in giá trị mật khẩu vào log hoặc tài liệu bàn giao.

File hợp đồng được tạo trước khi hoàn tất migration để chat Code triển khai song song. Migration `20261027181133_N5InitialCreate` đã áp dụng trên CSDL local. Kết quả CSDL, lệnh chạy và giới hạn thực tế ghi trong `database/N5_DATABASE_HANDOFF.md`.
