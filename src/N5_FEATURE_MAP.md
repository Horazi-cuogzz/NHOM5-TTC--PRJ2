# Bản đồ chức năng và mã nguồn RecruitHub N5

Ngày 28/10/2026. Các đường dẫn dưới đây tính từ thư mục src/RecruitHub.Web. Dùng để nối nội dung sơ đồ/báo cáo với mã nguồn đang chạy; chưa phân công cho từng sinh viên.

| SRS | Chức năng | Controller | Service hoặc ViewModel | Giao diện |
| --- | --- | --- | --- | --- |
| FR01 | Đăng ký, đăng nhập, đăng xuất | Controllers/N5AccountController.cs | ViewModels/N5AccountViewModel.cs; Services/N5IdentityErrorDescriber.cs | Views/N5Account/ |
| FR02 | Vai trò và quyền sở hữu | Authorize trong từng Controller; Program.cs | Điều kiện truy vấn theo UserId/EmployerUserId phía máy chủ | Views/Shared/N5Layout.cshtml |
| FR03 | Danh mục kỹ năng | Controllers/N5SkillsController.cs | N5SkillEditViewModel trong ViewModels/N5ProfileViewModel.cs | Views/N5Skills/ |
| FR04 | Hồ sơ ứng viên | Controllers/N5ProfileController.cs | ViewModels/N5ProfileViewModel.cs | Views/N5Profile/N5Edit.cshtml |
| FR05 | Thông tin công ty | Controllers/N5CompanyController.cs | N5CompanyViewModel trong ViewModels/N5ProfileViewModel.cs | Views/N5Company/N5Edit.cshtml |
| FR06 | Nháp, sửa, đăng, đóng tin | Controllers/N5EmployerJobsController.cs | Services/N5JobService.cs; ViewModels/N5JobViewModel.cs | Views/N5EmployerJobs/ |
| FR07 | Tìm, lọc, phân trang, chi tiết | Controllers/N5JobsController.cs | Services/N5JobService.cs | Views/N5Jobs/N5Index.cshtml; N5Details.cshtml |
| FR08 | Gửi đơn, chặn trùng | Controllers/N5JobsController.cs: Apply | Services/N5RecruitmentService.cs | Views/N5Jobs/N5Details.cshtml |
| FR09 | Đơn của ứng viên | Controllers/N5ApplicationsController.cs | ViewModels/N5ApplicationViewModel.cs | Views/N5Applications/ |
| FR10 | Danh sách đơn, đọc hồ sơ | Controllers/N5ApplicationsController.cs | Truy vấn giới hạn theo công ty đăng nhập | Views/N5Applications/ |
| FR11 | Chuyển trạng thái | Controllers/N5ApplicationsController.cs: ChangeStatus | Services/N5RecruitmentService.cs | Views/N5Applications/N5Details.cshtml |
| FR12 | Gợi ý theo kỹ năng | Controllers/N5JobsController.cs: Recommended; N5ApiController.cs | Services/N5MatchingService.cs; N5JobService.Card | Views/N5Jobs/N5Recommended.cshtml; Views/Shared/N5JobCard.cshtml |
| FR13 | Dashboard và API tổng quan | Controllers/N5DashboardController.cs; N5ApiController.cs | API lấy công ty từ claim đăng nhập | ClientApp/N5Dashboard.jsx; Views/N5Dashboard/N5Index.cshtml |

## Giao diện chung

Trang chủ: Controllers/N5HomeController.cs và Views/N5Home/N5Index.cshtml. Mẫu A dùng màu xanh dương, nền sáng, thẻ việc làm và khung kỹ năng. CSS: wwwroot/css/n5-site.css. JavaScript bổ trợ: wwwroot/js/n5-site.js. Biểu mẫu tìm việc: Views/Shared/N5Search.cshtml.

## Điểm vào dữ liệu

Models/N5*.cs là entity/enums. Data/N5RecruitHubContext.cs ánh xạ Identity và SQL Server. Data/N5DbSeeder.cs nạp dữ liệu mẫu trong Development. Migrations chứa phiên bản schema. Các file này do chat Database sở hữu; khi cần đổi schema, phối hợp qua hợp đồng dữ liệu.

## API

| Phương thức và đường dẫn | Vai trò | Phản hồi |
| --- | --- | --- |
| GET /api/recommendations/me?limit=10 | Candidate | Mảng jobId, title, companyName, matchedCount, requiredCount, scorePercent, matchedSkills, missingSkills. limit từ 1 đến 50. |
| GET /api/employer/summary | Employer | companyName, hasCompany, openJobs, totalApplications, byStatus. |

Cùng cookie đăng nhập với MVC; không nhận UserId hoặc CompanyId từ trình duyệt để xác định chủ sở hữu. Thao tác ghi dùng POST MVC có antiforgery.

