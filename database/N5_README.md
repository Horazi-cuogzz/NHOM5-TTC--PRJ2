# Khởi tạo cơ sở dữ liệu RecruitHub N5

CSDL phát triển `N5RecruitHub` đã được tạo tại `.\SQLEXPRESS` bằng Windows authentication. Bộ dữ liệu demo chỉ dùng cục bộ, thông tin người dùng và doanh nghiệp đều giả lập.

## Môi trường và cấu hình

- .NET SDK 10.0.401; SQL Server Express có instance `.\SQLEXPRESS` đang chạy.
- EF Core SQL Server, EF Design, ASP.NET Core Identity EF: 10.0.0. Tool dotnet-ef cục bộ trong `database/.config/dotnet-tools.json`: 10.0.0.
- Package trực tiếp System.Security.Cryptography.Pkcs 10.0.14 được dùng chung Web và tooling để thay dependency 9.0.4 có cảnh báo NuGet.
- ConnectionStrings:DefaultConnection: `Server=.\SQLEXPRESS;Database=N5RecruitHub;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`.
- UserSecretsId dùng chung: `N5RecruitHub-Local-Development`.

Mật khẩu demo đã được sinh ngẫu nhiên và lưu tại khóa `Seed:DemoPassword` trong user-secrets trên máy hiện tại. Mở Manage User Secrets của project Web trong IDE để xem giá trị khi cần đăng nhập. Không sao chép secrets.json vào repository. Seeder không đặt lại mật khẩu người dùng đã tồn tại.

Trên máy học tập khác, đặt `Seed:DemoPassword` bằng user-secrets hoặc `Seed__DemoPassword` trước lần đầu khởi tạo. Mật khẩu cần ít nhất 8 ký tự, có chữ hoa, chữ thường, chữ số và ký tự đặc biệt. Có thể đặt qua Terminal riêng với lệnh sau, thay giá trị minh họa bằng mật khẩu local của mình:

```powershell
dotnet user-secrets set "Seed:DemoPassword" "<mat-khau-demo-local>" --project "src/RecruitHub.Web/N5RecruitHub.Web.csproj"
```

## Khởi tạo bằng tooling độc lập

Chạy từ thư mục gốc dự án:

```powershell
dotnet run --project "database/N5.Tooling/N5.Database.Tooling.csproj" -- initialize
```

Công cụ dùng đúng entity, DbContext và migration của Web qua Compile Link, không sao chép model. Công cụ chỉ chấp nhận database tên N5RecruitHub, SQL Express cục bộ và Windows authentication. Nó chạy với Development, áp dụng migration rồi gọi N5DbSeeder.SeedAsync. Không cần Web đang chạy hoặc build xong giao diện.

Lần đầu tạo 3 role, 8 tài khoản, 2 công ty, 5 hồ sơ, 20 kỹ năng, 12 tin, 6 đơn. Seed dùng transaction và khóa ứng dụng để tránh hai lượt khởi tạo đồng thời. Sau khi có claim hoàn tất v1, các lần sau chỉ áp dụng migration mới, không thay đổi dữ liệu demo. Không xóa claim này để buộc tạo lại dữ liệu vì có thể gây trùng tin; việc tạo một bộ dữ liệu khác cần được xử lý riêng.

Nếu lần seed đầu thiếu mật khẩu hoặc dữ liệu không hợp lệ, transaction dữ liệu demo rollback; schema đã migrate vẫn được giữ. Sửa cấu hình rồi chạy lại cùng lệnh. Có thể xuất hiện cảnh báo savepoint khi chuỗi kết nối bật MARS: seeder không retry bên trong transaction và hủy toàn bộ transaction nếu gặp lỗi. Nếu không cần MARS, bên sở hữu cấu hình ứng dụng có thể bỏ tùy chọn này.

## Khởi tạo qua ứng dụng Web

Program gọi seed khi đồng thời ở Development và `Database:Initialize=true`. Ví dụ từ PowerShell tại gốc dự án:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:Database__Initialize = "true"
dotnet run --project "src/RecruitHub.Web/N5RecruitHub.Web.csproj"
```

Sau khi thoát ứng dụng, bỏ biến tạm nếu không muốn tự khởi tạo ở lần chạy sau:

```powershell
Remove-Item Env:Database__Initialize
```

Seeder kiểm tra môi trường Development và không tạo dữ liệu demo trong Production. Triển khai thật cần quy trình migration và tài khoản riêng, không dùng các tài khoản demo.

## Tài khoản và dữ liệu minh họa

| Vai trò | Email đăng nhập |
| --- | --- |
| Admin | admin@recruithub.example |
| Employer | employer1@recruithub.example, employer2@recruithub.example |
| Candidate | candidate1@recruithub.example đến candidate5@recruithub.example |

Các tài khoản mới dùng mật khẩu ở Seed:DemoPassword tại lần seed đầu. Mật khẩu được UserManager lưu dưới dạng hash chuẩn Identity.

Candidate 1 có C#, SQL Server, HTML, CSS và chưa có đơn. Tin .NET yêu cầu C#, SQL Server, Git cho điểm 2/3 = 66,67%; tin Frontend yêu cầu HTML, CSS, JavaScript, React cho điểm 2/4 = 50%; tin Python yêu cầu Python, Django cho điểm 0%. Tin 0% không thuộc danh sách gợi ý nhưng vẫn xuất hiện khi tìm việc thông thường.

Trong 12 tin có 8 Published còn hạn, 1 Published hết hạn, 2 Draft và 1 Closed tại thời điểm khởi tạo. Sáu đơn gồm 2 Submitted, 2 Reviewing, 1 Accepted, 1 Rejected; có đơn trên tin đóng và tin hết hạn. Deadline của demo tính từ thời điểm seed đầu, không được gia hạn khi chạy lại.

## Lệnh EF và script schema

Chạy các lệnh EF từ thư mục database để dùng đúng tool manifest:

```powershell
Set-Location database
dotnet tool restore
dotnet tool run dotnet-ef migrations list --project N5.Tooling/N5.Database.Tooling.csproj --context N5RecruitHubContext
dotnet tool run dotnet-ef database update --project N5.Tooling/N5.Database.Tooling.csproj --context N5RecruitHubContext
```

`database update` chỉ cập nhật schema, không seed tài khoản. `N5_SCHEMA.sql` là script schema idempotent đã sinh từ migration, không chứa mật khẩu hoặc dữ liệu demo. Nếu dùng SQLCMD, phải chỉ rõ CSDL N5RecruitHub đã được tạo:

```powershell
sqlcmd -S ".\SQLEXPRESS" -E -C -d N5RecruitHub -b -i N5_SCHEMA.sql
```

Không dùng EnsureCreated cùng Migration. Không sửa migration đã áp dụng; tạo migration tiếp theo khi thay đổi model. Khi Web build được, ưu tiên tạo migration trực tiếp vào Web bằng factory đã có:

```powershell
dotnet tool run dotnet-ef migrations add N5TenThayDoi --project ../src/RecruitHub.Web/N5RecruitHub.Web.csproj --context N5RecruitHubContext --output-dir Migrations --namespace RecruitHub.Web.Migrations
```

Nếu cần tạo migration bằng tooling trong lúc Web chưa build, dùng output-dir `../../src/RecruitHub.Web/Migrations`; kiểm tra cả migration và snapshot đều nằm trong thư mục Migrations của Web. Tooling liên kết lại cùng các file đó.

## Tài liệu bàn giao

- `N5_DATABASE_CONTRACT.md`: tên thuộc tính, navigation, enum và API seed cho chat Code.
- `N5_DATA_DICTIONARY.md`: kiểu SQL, khóa, quan hệ, CHECK và nguồn ERD cho chat Sơ đồ/Word.
- `N5_DATABASE_HANDOFF.md`: trạng thái bàn giao và kết quả xác nhận triển khai.

Factory thiết kế dùng IDesignTimeDbContextFactory để chạy EF mà không khởi động Program của Web, theo [tài liệu Microsoft về tạo DbContext lúc thiết kế](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation). Cập nhật dependency Pkcs dựa trên [thông báo bảo mật của Microsoft](https://github.com/dotnet/runtime/security/advisories/GHSA-846f-f764-76mx).
