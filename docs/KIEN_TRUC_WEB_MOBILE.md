# Giao diện web mobile trên develop

## Trạng thái hiện tại — 08/11/2026

Admin đã dùng MySQL và đăng nhập riêng; User vẫn dùng DemoShopDao, không kết luận checkout lưu MySQL. Solution còn nhiều project. Xem ADMIN_REPORT_CURRENT.md và progress/AI_HANDOFF.md.

Build solution: 0 lỗi, 0 cảnh báo. 19 smoke checks User còn lại pass (không phải test Admin). HTTP đăng nhập đúng/sai, thiếu CSRF, chặn ẩn danh và đăng xuất đều đạt. DAO chỉ đọc MySQL: 10 đơn + 10 chi tiết, 10 hóa đơn + 10 chi tiết, 10 phiếu nhập + 10 chi tiết, 10 dòng tồn kho tổng 485; 10 chương trình/10 voucher. Doanh thu hóa đơn đã thanh toán năm 2026: 114.100.000 đồng trên dữ liệu mẫu tại thời điểm kiểm tra.

Chưa kiểm thử ghi CRUD, lập/thu tiền hóa đơn, cộng/trừ tồn, rollback hoặc concurrency trên database riêng. Chưa áp dụng migration DON_HANG.MaKho. Chưa nghiệm thu toàn bộ đồ án, UI tương tác hoặc máy sạch. Không coi build/HTTP 200/dữ liệu giả là bằng chứng nghiệp vụ ghi MySQL.

## Tài liệu ghép GUI trước đây (lịch sử)

Bản giao diện được gộp từ `feature/web-mobile-gui` vào `develop`, giữ nguyên toàn bộ các file và thư mục đã pull từ `origin/develop`. Mã ứng dụng đặt theo sườn nhánh `main`: giao diện trong `GUI/user`, `GUI/admin`, component dùng chung trong `GUI/shared`, nghiệp vụ trong `BUS`, truy cập dữ liệu trong `DAO` và mô hình dữ liệu trong `DTO`. Không sửa nội dung README, phân công hoặc checklist gốc của nhóm.

## Chạy ứng dụng

Yêu cầu .NET 10 SDK. Mở `SmartShop.slnx`, chọn `SmartShop.User` làm startup project, hoặc chạy:

```powershell
dotnet run --project GUI/user/SmartShop.User.csproj --launch-profile http --urls http://localhost:5190
```

Mở http://localhost:5190. Admin ở `/admin`; khung xem điện thoại ở `/mobile-preview.html`. Admin được nạp vào cùng ứng dụng, không cần chạy máy chủ thứ hai.

## File bổ sung theo sườn mới

| Vị trí | Nội dung |
|---|---|
| GUI/user/Components/Pages | Trang chủ, sản phẩm, giỏ hàng, thanh toán, đơn, tài khoản, trợ giúp |
| GUI/user/Components/Layout | Header, menu trái, thanh điều hướng mobile và popup |
| GUI/user/wwwroot | CSS, minh họa thiết bị và tài nguyên giao diện |
| GUI/shared | State và lớp điều khiển UI dùng chung cho User và Admin |
| GUI/admin/Components/Pages/Admin.razor | Quản trị sản phẩm, đơn hàng, thống kê demo |
| GUI/admin/SmartShop.Admin.csproj | Thư viện Razor chứa màn hình quản trị |
| DTO | Model Product, Order, OrderLine |
| BUS | Tìm kiếm ArrayList, tính tiền, giỏ hàng, kiểm tra dữ liệu |
| DAO | Kho dữ liệu demo theo phiên |
| tools/SmartShop.SmokeTests | Kiểm tra nghiệp vụ không cần thư viện test ngoài |

GUI gọi BUS; BUS gọi DAO; DTO dùng để trao đổi dữ liệu. User và Admin dùng chung service scoped trong phiên Blazor. Router và endpoint máy chủ khai báo thêm assembly Admin để `/admin` hoạt động khi truy cập trực tiếp và khi chuyển trang.

## Kiểm tra

```powershell
dotnet build SmartShop.slnx
dotnet run --project tools/SmartShop.SmokeTests
```

## Phạm vi

Giao diện vẫn dùng dữ liệu demo trong bộ nhớ. Tải lại hoặc mở phiên mới có thể mất giỏ và đơn; Admin chưa có xác thực. Chưa kết nối SQL Server, thanh toán thật hoặc app MAUI riêng. Hình thiết bị là minh họa CSS. Tài liệu gốc có thể còn nhắc WinForms; tài liệu này ghi riêng phần web/mobile đã được người dùng yêu cầu, không ghi đè tài liệu gốc.

## Git

Làm việc và push trên `develop`. Lệnh push dùng đích rõ ràng `HEAD:develop`; không push `main`.
