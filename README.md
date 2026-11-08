# RecruitHub - Cổng tuyển dụng và quản lý ứng viên cho doanh nghiệp IT

> **Đề tài DT06** — Dự án thực tập chuyên đề / thực tập công nghệ (TTCĐ-PRJ2)  
> **Nhóm thực hiện:** Nhóm 5  
> **Repository:** [https://github.com/Horazi-cuogzz/NHOM5-TTC--PRJ2](https://github.com/Horazi-cuogzz/NHOM5-TTC--PRJ2)

---

## 👥 Thành viên nhóm & Phân công

| STT | Họ và tên | Mã sinh viên | Vai trò | Tài khoản GitHub |
| :---: | :--- | :---: | :---: | :---: |
| 1 | **Phùng Quang Cường** | 2410900015 | Trưởng nhóm | [@Horazi-cuogzz](https://github.com/Horazi-cuogzz) |
| 2 | **Nguyễn Huy Anh** | 2410900003 | Thành viên | [@Hyan-we](https://github.com/Hyan-we) |
| 3 | **Nguyễn Tùng Dương** | 2410900017 | Thành viên | [@duongnavia2006-a11y](https://github.com/duongnavia2006-a11y) |

---

## 🎯 Giới thiệu đề tài

**RecruitHub** là giải pháp cổng tuyển dụng trực tuyến chuyên biệt cho lĩnh vực Công nghệ Thông tin (IT). Hệ thống kết nối ứng viên và nhà tuyển dụng, tự động hóa quy trình nộp - duyệt hồ sơ và hỗ trợ gợi ý việc làm thông minh dựa trên độ trùng khớp kỹ năng (Skill Matching Algorithm).

### Tính năng nổi bật
* **Quản lý danh mục kỹ năng (Skills Catalog):** Phân loại kỹ năng IT theo nhóm (Frontend, Backend, Database, Cloud/DevOps...).
* **Hồ sơ ứng viên (Candidate Profile):** Ứng viên cập nhật thông tin cá nhân, kinh nghiệm làm việc và danh mục kỹ năng thực tế.
* **Đăng tin tuyển dụng (Job Postings):** Doanh nghiệp tạo và quản lý tin tuyển dụng với yêu cầu kỹ năng cụ thể, mức lương, địa điểm và hạn nộp hồ sơ.
* **Gợi ý việc làm thông minh (Smart Skill Matching):** Tính toán tỷ lệ phần trăm phù hợp giữa kỹ năng của ứng viên và yêu cầu công việc; trực quan hóa các kỹ năng đã đáp ứng và kỹ năng còn thiếu.
* **Quy trình ứng tuyển đa giai đoạn:** Ứng viên nộp hồ sơ kèm thư giới thiệu; Nhà tuyển dụng quản lý và chuyển đổi trạng thái hồ sơ (*Chờ duyệt $\rightarrow$ Đã duyệt $\rightarrow$ Phỏng vấn $\rightarrow$ Nhận việc / Từ chối*).
* **Bảng điều khiển trực quan (React Dashboard):** Thống kê số liệu thời gian thực về tin đăng, ứng viên và tỷ lệ chuyển đổi trạng thái tuyển dụng.

---

## 🛠️ Công nghệ sử dụng

* **Backend:** ASP.NET Core MVC (.NET 10), C# 13, Entity Framework Core 10 (Code First, Migrations).
* **Bảo mật & Xác thực:** ASP.NET Core Identity (Role-based Authorization: *Admin*, *Employer*, *Candidate*).
* **Cơ sở dữ liệu:** Microsoft SQL Server (Local Express: `.\SQLEXPRESS`, Database: `N5RecruitHub`).
* **Frontend:** Razor Pages/Views, Bootstrap 5 (chủ đề xanh dương N5), jQuery, Vite & React 19 (Dashboard).
* **Quản lý phiên bản:** Git & GitHub.

---

## 📁 Cấu trúc thư mục kho lưu trữ

```text
TTCĐ-PRJ2/
├── README.md                           # Tài liệu tổng quan dự án (tệp này)
├── .gitignore                          # Cấu hình bỏ qua tệp biên dịch, rác, secrets
├── KE_HOACH_DU_AN_RECRUITHUB.md        # Kế hoạch tổng thể và lộ trình 8 tuần
│
├── Bai_Tuan_1/                         # Tài liệu báo cáo Tuần 1 (Đề cương, Kế hoạch, SRS)
│   ├── Nhom5-DT06-01-DeCuong.docx
│   ├── Nhom5-DT06-02-KeHoach-8Tuan.docx
│   └── Nhom5-DT06-03-DacTa-YeuCau-SRS.docx
│
├── Bai_Tuan_2/                         # Báo cáo tiến độ Tuần 2 (Thiết kế & Nền tảng)
│   └── Nhom5-DT06-BaoCao-Tuan2.docx
│
├── Bai_Tuan_3/                         # Báo cáo tiến độ Tuần 3 (Hồ sơ, Tin tuyển dụng & CSDL)
│   └── Nhom5-DT06-BaoCao-Tuan3.docx
│
├── src/                                # Mã nguồn ứng dụng chính
│   ├── N5RecruitHub.sln                # Visual Studio Solution
│   ├── N5Start.ps1                     # Script PowerShell khởi động 1 chạm
│   ├── N5_README.md                    # Hướng dẫn chạy và cấu trúc module Web
│   └── RecruitHub.Web/                 # Project ASP.NET Core MVC & React
│       ├── Controllers/                # Controllers xử lý nghiệp vụ
│       ├── Models/                     # Entity models EF Core
│       ├── Data/                       # DbContext & Seeder dữ liệu mẫu
│       ├── Services/                   # Nghiệp vụ tính matching, quản lý job, tuyển dụng
│       ├── Views/                      # Giao diện Razor Views
│       ├── Migrations/                 # EF Core Migrations
│       ├── ClientApp/                  # Frontend React (Vite) cho Dashboard
│       └── wwwroot/                    # Static assets (CSS, JS, Libs)
│
├── database/                           # Quản lý & Khởi tạo cơ sở dữ liệu
│   ├── N5_SCHEMA.sql                   # Mã nguồn SQL tạo bảng và ràng buộc
│   ├── N5_DATA_DICTIONARY.md           # Từ điển dữ liệu chi tiết
│   ├── N5_DATABASE_CONTRACT.md         # Quy chuẩn hợp đồng CSDL
│   ├── N5_DATABASE_HANDOFF.md          # Biên bản bàn giao kỹ thuật CSDL
│   ├── N5_README.md                    # Hướng dẫn khởi tạo CSDL bằng Tooling
│   └── N5.Tooling/                     # Công cụ .NET Console khởi tạo & seed dữ liệu
│
├── docs/                               # Tài liệu phân tích, đặc tả Use Case & Sơ đồ
│   └── diagrams/                       # Sơ đồ UC-TQ, UC01 - UC09 (.drawio, .png, .svg)
│
└── output/                             # Ảnh chụp màn hình & thiết kế concept giao diện
```

---

## 🚀 Hướng dẫn cài đặt và chạy ứng dụng

### Yêu cầu môi trường
* **.NET SDK 10.0** (hỗ trợ C# 13).
* **Microsoft SQL Server Express** (instance `.\SQLEXPRESS` đang chạy).
* **Node.js** (khuyến nghị v20+ hoặc v24) & **npm**.
* **PowerShell** (Windows).

### Khởi chạy nhanh (Khuyến nghị)
Chạy script tự động tại thư mục gốc của dự án:

```powershell
.\src\N5Start.ps1
```

* Script sẽ tự động cài đặt gói frontend (`npm ci`), biên dịch React Dashboard (`npm run build`) và khởi chạy ứng dụng web tại:  
  👉 **`http://localhost:5055`**

---

## 🗄️ Khởi tạo cơ sở dữ liệu & Dữ liệu mẫu (Database Seeding)

### 1. Cấu hình bảo mật mật khẩu Demo (User Secrets)
Hệ thống sử dụng .NET User Secrets để quản lý mật khẩu khởi tạo an toàn, **không lưu mật khẩu vào mã nguồn**:

```powershell
dotnet user-secrets set "Seed:DemoPassword" "<MatKhauDemoCuaBan_ItNhat8KyTu>" --project "src/RecruitHub.Web/N5RecruitHub.Web.csproj"
```

### 2. Khởi tạo CSDL
Chạy lệnh khởi tạo kèm seed dữ liệu:

* **Cách 1 (Qua script khởi động):**
  ```powershell
  .\src\N5Start.ps1 -InitializeDatabase
  ```

* **Cách 2 (Qua công cụ độc lập trong database tooling):**
  ```powershell
  dotnet run --project "database/N5.Tooling/N5.Database.Tooling.csproj" -- initialize
  ```

### 3. Tài khoản mẫu được tạo sẵn
Hệ thống đã seed sẵn dữ liệu demo để kiểm thử ngay:
* **Quản trị viên (Admin):** `admin@recruithub.example`
* **Nhà tuyển dụng (Employer):** `employer1@recruithub.example`, `employer2@recruithub.example`
* **Ứng viên (Candidate):** `candidate1@recruithub.example` đến `candidate5@recruithub.example`
* *(Mật khẩu đăng nhập tương ứng với giá trị đã đặt trong `Seed:DemoPassword`).*

---

## 📜 Quy ước đóng góp & Commit

Dự án áp dụng quy ước tiền tố commit rõ ràng:
* `[Docs]`: Cập nhật tài liệu, kế hoạch, đặc tả và báo cáo tuần.
* `[Chore]`: Cấu hình kho Git, `.gitignore`, build scripts.
* `[DB]`: Schema CSDL, migrations, data dictionary, seeder.
* `[Feat]`: Thêm mới hoặc cải tiến tính năng mã nguồn MVC / React.
* `[Fix]`: Sửa lỗi ứng dụng.
