# IV. Hướng dẫn cài đặt và chạy

Bản nháp cập nhật 08/11/2026. Mã hiện tại là .NET 10 Razor/Blazor + MySQL, không phải WinForms/SQL Server trong kế hoạch ban đầu.

## Công cụ và cấu trúc

.NET 10 SDK, MySQL đang chạy, Visual Studio hoặc VS Code. MySql.Data 26.10.0 theo csproj. SmartShop.slnx hiện có User (host), Admin (Razor), UI.Shared, BUS, DAO và DTO; chưa gom về một project.

GUI/admin chứa giao diện/xác thực; BUS/admin chứa validation/nghiệp vụ; DAO/admin chứa SQL có tham số; DTO chứa dữ liệu/hợp đồng. GUI/user là host dùng chung, luồng mua hàng User không được thay trong đợt Admin.

## Cấu hình

Database quan_ly_cua_hang_dien_tu tại 127.0.0.1:3306. Nhập ConnectionStrings:SmartShop trong Manage User Secrets của SmartShop.User (UserSecretsId SmartShop.Admin.Local). SMARTSHOP_DB_CONNECTION là fallback. Không commit/chụp chuỗi thật chứa mật khẩu.

Không chạy lại schema gốc chứa DROP TABLE. Migration MaKho đang chờ duyệt. Đăng nhập Admin không cần bảng tài khoản; thông tin đăng nhập giữ riêng, có thể override Admin:Username/Admin:PasswordHash trong User Secrets.

## Chạy

```powershell
dotnet build SmartShop.slnx
dotnet run --project GUI/user/SmartShop.User.csproj --launch-profile http
```

HTTP http://localhost:5264, mở /admin/login. HTTPS profile dùng https://localhost:7040. Nếu DLL bị khóa, dừng phiên ứng dụng cũ trước khi build. Chưa chạy thử trên máy sạch.

## Kiểm chứng

Build solution: 0 lỗi, 0 cảnh báo. 19 smoke checks User còn lại pass (không phải test Admin). HTTP đăng nhập đúng/sai, thiếu CSRF, chặn ẩn danh và đăng xuất đều đạt. DAO chỉ đọc MySQL: 10 đơn + 10 chi tiết, 10 hóa đơn + 10 chi tiết, 10 phiếu nhập + 10 chi tiết, 10 dòng tồn kho tổng 485; 10 chương trình/10 voucher. Doanh thu hóa đơn đã thanh toán năm 2026: 114.100.000 đồng trên dữ liệu mẫu tại thời điểm kiểm tra.

Chưa kiểm thử ghi CRUD, lập/thu tiền hóa đơn, cộng/trừ tồn, rollback hoặc concurrency trên database riêng. Chưa áp dụng migration DON_HANG.MaKho. Chưa nghiệm thu toàn bộ đồ án, UI tương tác hoặc máy sạch. Không coi build/HTTP 200/dữ liệu giả là bằng chứng nghiệp vụ ghi MySQL.
