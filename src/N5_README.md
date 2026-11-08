# RecruitHub Nhóm 5

Ứng dụng tuyển dụng IT của Nhóm 5, DT06. Giao diện tiếng Việt theo mẫu A xanh dương đã được chọn; tên các tệp chức năng dùng N5. Phiên bản hiện tại chạy cục bộ với SQL Server, Identity, MVC/Razor, Bootstrap, jQuery và dashboard React.

## Mở sản phẩm trên máy hiện tại

Chạy từ thư mục dự án trong PowerShell:

```powershell
.\src\N5Start.ps1
```

Mở http://localhost:5055. Nhấn Ctrl+C trong cửa sổ chạy để dừng. Có thể đổi cổng bằng `-Port 5056`.

Mở solution `src/N5RecruitHub.sln` bằng Visual Studio có hỗ trợ .NET 10, hoặc mở thư mục với VS Code. Project chính: `src/RecruitHub.Web/N5RecruitHub.Web.csproj`.

## Chuẩn bị trên máy khác

1. Cài .NET SDK 10, SQL Server và Node.js tương thích Vite 8 (máy phát triển dùng Node 24).
2. Cập nhật `ConnectionStrings:DefaultConnection` bằng user-secrets của project theo SQL Server cục bộ. File Development hiện dùng Windows authentication với `.\\SQLEXPRESS` và CSDL `N5RecruitHub`.
3. Tạo mật khẩu demo trong user-secrets của project bằng Manage User Secrets, khóa `Seed:DemoPassword`. Mật khẩu cần ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt. Không đưa tệp này vào Git.
4. Chạy `.\src\N5Start.ps1 -InitializeDatabase` một lần. Seeder áp dụng migration và bổ sung dữ liệu mẫu; các lần sau chỉ chạy `.\src\N5Start.ps1`.

Script khởi động cài frontend bằng `npm ci` khi cần và build React trước khi chạy Web. Nếu muốn làm từng bước:

```powershell
cd src/RecruitHub.Web/ClientApp
npm ci
npm run build
cd ..
dotnet run --launch-profile http
```

Thư viện Bootstrap/jQuery được sao chép vào wwwroot/lib; dashboard được build vào wwwroot/dashboard. Website không phụ thuộc CDN khi sử dụng.

## Tài khoản sử dụng

Có thể tự đăng ký vai trò Ứng viên hoặc Nhà tuyển dụng trên website. Quản trị viên được seed, không được chọn khi tự đăng ký.

| Vai trò | Email mẫu |
| --- | --- |
| Ứng viên | candidate1@recruithub.example đến candidate5@recruithub.example |
| Nhà tuyển dụng | employer1@recruithub.example, employer2@recruithub.example |
| Quản trị viên | admin@recruithub.example |

Mật khẩu mẫu đã được chat Database sinh và lưu ngoài repository, trong user-secrets ID `N5RecruitHub-Local-Development`, khóa `Seed:DemoPassword`. Xem bằng Manage User Secrets của project. Các lần seed sau không đổi lại mật khẩu.

Sau khi chạy thử luồng tạo tài khoản và ứng tuyển, máy phát triển có thêm hai tài khoản giả lập, công ty N5 Studio Demo, một tin đã đóng và một đơn đạt yêu cầu. Chúng không thuộc dữ liệu seed cố định và không tự xuất hiện trên máy mới.

## Luồng sử dụng

- Ứng viên đăng ký, hoàn thiện hồ sơ/kỹ năng, tìm hoặc xem việc làm phù hợp, mở chi tiết rồi gửi đơn. Các đơn đã gửi nằm trong menu riêng.
- Nhà tuyển dụng cập nhật công ty, tạo bản nháp và đăng tin. Trong mục Ứng viên, mở đơn và chuyển Đã nộp → Đang xem xét → Đạt yêu cầu hoặc Từ chối.
- Quản trị viên thêm, sửa hoặc ngừng sử dụng kỹ năng. Quan hệ với hồ sơ/tin cũ được giữ.
- Dashboard hiển thị tin còn mở và đơn theo trạng thái của đúng công ty đang đăng nhập.

Hạn nhận hồ sơ là hết ngày theo giờ Việt Nam, lưu UTC. Đơn tham chiếu hồ sơ hiện tại. Tin đã có đơn giữ nguyên bộ kỹ năng. Không xóa tin, không mở lại tin đã đóng, không rút/nộp lại đơn.

Điểm phù hợp = số kỹ năng trùng / số kỹ năng yêu cầu × 100. Xếp theo điểm chưa làm tròn giảm dần, thời điểm đăng giảm dần, ID tăng dần. Gợi ý chỉ gồm tin còn tuyển, có điểm lớn hơn 0 và chưa ứng tuyển. Tỷ lệ này không phải xác suất được tuyển dụng.

## Cấu trúc và trách nhiệm

- Controllers: tiếp nhận yêu cầu, kiểm tra vai trò và điều phối.
- Services: tìm việc, matching, quy tắc gửi đơn/trạng thái, định dạng thời gian.
- ViewModels: dữ liệu biểu mẫu và hiển thị, không tạo bộ entity khác.
- Views: Razor tiếng Việt; layout chung N5Layout.cshtml.
- ClientApp: React/Vite cho tổng quan.
- Models, Data, Migrations: đầu ra do chat Database cung cấp và quản lý.
- [Bản đồ chức năng và file](N5_FEATURE_MAP.md).
- [Hợp đồng Database](../database/N5_DATABASE_CONTRACT.md) và [hướng dẫn Database](../database/N5_README.md).

Program.cs, appsettings*.json, _ViewImports.cshtml, _ViewStart.cshtml, package.json, package-lock.json giữ quy ước công cụ/framework. File migration giữ timestamp do EF sinh; phần tên migration có N5. Thư viện bên thứ ba giữ tên và giấy phép gốc.

## Mốc triển khai ngày 28/10/2026

Đã build ứng dụng và React; chạy qua luồng đăng ký → hồ sơ/công ty → tạo/đăng tin → gửi đơn → xem xét → đạt yêu cầu → đóng tin. Gửi lặp không sinh thêm đơn, tin đã nộp được loại khỏi gợi ý, tài khoản công ty khác không đọc được đơn hoặc sửa tin. API trả 401 khi chưa đăng nhập và 403 khi sai vai trò. Đây là xác nhận chạy thực tế phục vụ triển khai, không phải bộ test case hay chương kiểm thử.

Chưa triển khai hosting công khai. Báo cáo, sơ đồ và phân công sinh viên thuộc các bước/chat riêng. Bản này không bổ sung tải CV, email, chat, thanh toán hoặc lịch phỏng vấn.

Tài liệu kỹ thuật dùng khi tích hợp: [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity?view=aspnetcore-10.0), [EF Core SQL Server](https://learn.microsoft.com/ef/core/providers/sql-server/).

