# Bàn giao Database RecruitHub N5

Ngày 28/10/2026. Phần Database đã triển khai theo SRS tuần 1 và hợp đồng tiền tố N5 do chat Code cung cấp.

## Trạng thái bàn giao

- Đã tạo và migrate CSDL phát triển **N5RecruitHub** trên **.\SQLEXPRESS**, Windows authentication. Không thao tác sửa/xóa CSDL khác.
- Migration hiện hành: **20261027181133_N5InitialCreate**, EF Core 10.0.0. Timestamp migration do EF sinh theo UTC; ngày bàn giao hiển thị theo giờ Việt Nam.
- Có 7 bảng nghiệp vụ N5, 7 bảng Identity và __EFMigrationsHistory. Tám entity bao gồm N5AppUser ánh xạ AspNetUsers, không phải 8 bảng N5 riêng.
- Dữ liệu mẫu: **3 role, 8 tài khoản, 2 công ty, 5 hồ sơ, 20 kỹ năng, 12 tin, 6 đơn**. Thông tin đều giả lập.
- Mật khẩu demo đã được sinh ngẫu nhiên, lưu ngoài repo ở **Seed:DemoPassword** của user-secrets **N5RecruitHub-Local-Development**, dùng chung Web/tooling; không ghi vào code, SQL, tài liệu hay log. Xem Manage User Secrets của project khi cần đăng nhập.

## File do chat Database tạo

| Phạm vi | Nội dung |
| --- | --- |
| src/RecruitHub.Web/Models/ | N5AppUser, N5CandidateProfile, N5Company, N5Skill, N5CandidateSkill, N5JobPosting, N5JobSkill, N5Application, N5JobStatus, N5ApplicationStatus |
| src/RecruitHub.Web/Data/ | N5RecruitHubContext.cs, N5DesignTimeContextFactory.cs, N5DbSeeder.cs |
| src/RecruitHub.Web/Migrations/ | 20261027181133_N5InitialCreate.cs, file Designer và N5RecruitHubContextModelSnapshot.cs |
| database/N5.Tooling/ | Project console N5.Database.Tooling.csproj và N5Program.cs, liên kết cùng model/data/migration của Web |
| database/.config/ | dotnet-tools.json cố định dotnet-ef 10.0.0 |
| database/ | N5_DATABASE_CONTRACT.md, N5_DATA_DICTIONARY.md, N5_README.md, N5_SCHEMA.sql, N5_DATABASE_HANDOFF.md và .gitignore cục bộ |

Không sửa csproj/Program/appsettings, Controller, View, ViewModel, Service hoặc các đầu ra của chat khác. Không tạo kiểm thử, test case, chương kiểm thử, slide, phân công sinh viên; không commit hoặc push.

## Hợp đồng tích hợp đã có

Context `RecruitHub.Web.Data.N5RecruitHubContext`; entity/enum tại `RecruitHub.Web.Models`. DbSet: CandidateProfiles, Companies, Skills, CandidateSkills, JobPostings, JobSkills, Applications; tài khoản dùng Users của Identity. Chi tiết trường/navigation và giới hạn null nằm trong N5_DATABASE_CONTRACT.md.

`N5DbSeeder.SeedAsync(IServiceProvider services, IConfiguration configuration)` tự tạo scope, migrate, khóa seed bằng sp_getapplock và tạo demo trong transaction. Program gọi khi Development và Database:Initialize=true. Seeder cũng kiểm tra hai điều kiện này. Claim N5RecruitHub:DemoSeed=v1 đánh dấu đã hoàn tất; chạy lại không thay đổi dữ liệu demo và không đặt lại mật khẩu. Seed không tự khôi phục demo đã bị xóa hoặc chỉnh sửa sau lần đầu.

Quan hệ tài khoản–hồ sơ/công ty có unique FK; hai bảng kỹ năng có PK ghép; đơn có unique cặp JobPostingId–CandidateProfileId. Chín FK nghiệp vụ đều NO ACTION. CHECK xác nhận enum, tên kỹ năng không rỗng, PublishedAt của tin đã đăng/đóng và offset UTC. NormalizedName tự đồng bộ khi SaveChanges. Không lưu điểm matching hoặc snapshot hồ sơ.

Chat Code chịu trách nhiệm điều kiện đăng/nộp, quyền và chủ sở hữu, chuyển trạng thái, giữ bộ kỹ năng tin có đơn, không chọn mới kỹ năng inactive và giới hạn nhập liệu. Chat Sơ đồ/Word có thể dùng bảng và nguồn Mermaid trong N5_DATA_DICTIONARY.md; các nội dung này đã đối chiếu schema thực tế.

## Kết quả xác nhận triển khai

- Tooling và Web build thành công với **0 lỗi, 0 cảnh báo** tại mốc bàn giao.
- `migrations has-pending-model-changes` trên cả tooling và Web trả về không có thay đổi model so với migration.
- Migration đã xuất hiện trong __EFMigrationsHistory; các PK/UQ/FK/CHECK nghiệp vụ có mặt, FK/CHECK không bị disable hoặc đánh dấu not trusted.
- Script N5_SCHEMA.sql đã chạy lại thành công trên N5RecruitHub đã có migration, không tạo trùng schema.
- Chạy seed lần thứ hai giữ nguyên 8 tài khoản, 2 công ty, 5 hồ sơ, 20 kỹ năng, 12 tin và 6 đơn; checksum của tin/đơn trước và sau giống nhau, mốc seed vẫn là một claim.
- Truy vấn dữ liệu của candidate1 với ba tin mẫu SRS trả về lần lượt **2/3 = 66,67%**, **2/4 = 50%**, **0/2 = 0%**.
- Tại lần khởi tạo có 8 tin còn tuyển, 1 tin Published hết hạn, 2 nháp và 1 đóng; các đơn gồm 2 Submitted, 2 Reviewing, 1 Accepted, 1 Rejected.

Các xác nhận trên phục vụ hoàn thiện/tích hợp phần Database, không phải bộ kiểm thử hoặc đánh giá hoàn tất toàn bộ nghiệp vụ của Web.

## Phiên bản và lưu ý vận hành

EF Core SQL Server, Design, Identity EF và dotnet-ef giữ **10.0.0**. Restore ban đầu phát hiện NU1902/NU1903 từ dependency System.Security.Cryptography.Pkcs 9.0.4. Chat Code đã chọn tham chiếu trực tiếp **10.0.14** cho Web; tooling Database đồng bộ **10.0.14**. Không tắt audit và không cần đổi schema vì thay dependency này.

Chuỗi kết nối hiện bật MARS, nên seed lần đầu có cảnh báo EF về savepoint. Seeder không tiếp tục/retry sau lỗi bên trong transaction; lỗi làm hủy toàn bộ dữ liệu demo của lượt đó. Có thể bỏ MARS ở cấu hình ứng dụng nếu không có nhu cầu sử dụng; chat Database không sửa appsettings do Code sở hữu.

Mốc hạn tin demo tương đối với lần seed đầu, không tự gia hạn ở lượt sau. Ngày nhập hạn phải chuyển từ cuối ngày giờ Việt Nam sang UTC ở Service. Dùng N5_README.md để khởi tạo trên máy khác và N5_SCHEMA.sql khi cần script schema; script không tạo dữ liệu demo.

## Bước tiếp theo của chat Code

Tiếp tục tích hợp các chức năng lên schema đã có; dùng tài khoản demo và cấu hình user-secrets chung. Không tạo model/DbContext thứ hai hoặc dùng EnsureCreated. Nếu cần đổi cấu trúc dữ liệu, cập nhật hợp đồng trước và tạo migration mới trong phạm vi sở hữu Database. Phần giao diện, controller và nghiệp vụ vẫn thuộc chat Code.
