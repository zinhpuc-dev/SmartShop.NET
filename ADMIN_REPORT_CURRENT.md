# SmartShop Admin — triển khai và kiểm chứng

Cập nhật **08/11/2026**. Trạng thái này thay thế kết luận cũ về DAO demo Admin và đăng nhập cần bảng tài khoản.

## Phạm vi hiện tại

.NET 10 Razor/Blazor, MySql.Data; GUI → BUS → hợp đồng DTO → DAO MySQL. Chỉ sửa Admin. Hai file host Program.cs/Routes.razor bổ sung xác thực Admin, không sửa giỏ hàng/checkout User. User vẫn dùng DemoShopDao; không khẳng định User đặt đơn đã lưu MySQL. Solution vẫn gồm các project riêng, chưa gom toàn bộ về một project/folder.

## Các module

| Module | Đã triển khai | Kiểm chứng / giới hạn |
|---|---|---|
| Đăng nhập | Một Admin cục bộ, PasswordHasher, cookie HttpOnly, CSRF, giới hạn đăng nhập, session/đăng xuất | HTTP đúng/sai/ẩn danh/CSRF/đăng xuất đạt; không cần bảng tài khoản |
| Khách hàng | CustomerDAO MySQL, CRUD/validation/tìm kiếm/Reload cache | Đọc 10 hồ sơ; chưa thử ghi/xóa |
| Nhân viên | EmployeeDAO, ChucVu, CRUD/tìm kiếm | Đọc 10 hồ sơ; chưa thử ghi, schema không có địa chỉ/tình trạng |
| Nhà cung cấp | SupplierDAO, CRUD/tìm kiếm và dùng trong nhập hàng | Đọc 10 NCC; chưa thử ghi/xóa |
| Sản phẩm/loại | Thêm/sửa/ngừng bán, lọc, mã loại/trạng thái, tồn theo kho | Đọc 10 sản phẩm; chưa có CRUD đơn vị/ảnh, tên danh mục/ngưỡng tồn |
| Kho/nhập hàng | Sửa loaded; tồn theo kho/chi tiết phiếu; mã ghi phiếu+chi tiết+cộng tồn cùng transaction | Đọc 10 phiếu/10 chi tiết, tồn tổng 485; chưa thử ghi/rollback |
| Đơn hàng | Sửa List trong foreach; đọc chi tiết, kiểm tra chuyển trạng thái, transaction trừ tồn | Đọc 10 đơn/10 chi tiết; MaKho chưa được áp dụng, trừ tồn chưa thử |
| Hóa đơn | Sửa List trong foreach; lập từ đơn đã giao/hoàn thành; xác nhận thu tiền | Đọc 10 hóa đơn/10 chi tiết; chưa thử lập/thu tiền |
| Báo cáo | Khoảng ngày, chỉ hóa đơn đã thanh toán theo ngày hóa đơn | Năm 2026: 114.100.000 đồng, 10 hóa đơn; chưa có chi/lợi nhuận/ma trận quý |
| Khuyến mại/voucher | Chuẩn hóa PhanTram/SoTien, kiểm tra thời gian/chương trình/số lượt dùng | Đọc 10 chương trình/10 voucher; chưa thử ghi hoặc áp dụng toàn bộ giảm giá bán hàng |

## Kiểm tra 08/11/2026

Build solution: 0 lỗi, 0 cảnh báo. 19 smoke checks User còn lại pass (không phải test Admin). HTTP đăng nhập đúng/sai, thiếu CSRF, chặn ẩn danh và đăng xuất đều đạt. DAO chỉ đọc MySQL: 10 đơn + 10 chi tiết, 10 hóa đơn + 10 chi tiết, 10 phiếu nhập + 10 chi tiết, 10 dòng tồn kho tổng 485; 10 chương trình/10 voucher. Doanh thu hóa đơn đã thanh toán năm 2026: 114.100.000 đồng trên dữ liệu mẫu tại thời điểm kiểm tra.

Kiểm tra HTTP dùng host thử ở cổng riêng; kiểm tra DAO là chỉ đọc trực tiếp, không phải toàn bộ thao tác trình duyệt/Blazor. Host thử đã dừng, công cụ C# tạm ngoài repo đã xóa. Không dùng Computer Use.

## Dọn test

Đã xóa 5 file CustomerChecks/EmployeeChecks/WarehouseChecks/OrderManagementChecks/PromotionChecks và 3 DemoCustomer/Employee/SupplierRepository chỉ dùng cho test Admin. Đã gỡ các lời gọi Admin và test NCC trong runner dùng chung. tools/SmartShop.SmokeTests hiện giữ 19 checks User/dữ liệu giả. Không xóa DemoShopDao vì User vẫn sử dụng.

## An toàn và phần chưa xong

Chưa kiểm thử ghi CRUD, lập/thu tiền hóa đơn, cộng/trừ tồn, rollback hoặc concurrency trên database riêng. Chưa áp dụng migration DON_HANG.MaKho. Chưa nghiệm thu toàn bộ đồ án, UI tương tác hoặc máy sạch. Không coi build/HTTP 200/dữ liệu giả là bằng chứng nghiệp vụ ghi MySQL.

- Không chạy INSERT/UPDATE/DELETE hoặc migration trong đợt sửa/kiểm tra này.
- Database/QuanLyCuaHangDienTu_MySQL.sql có DROP TABLE: không chạy lại trên database hiện có.
- Database/migrations/2026-03-Admin-Auth-Order-Warehouse.sql hiện chỉ thêm MaKho/index/FK vào DON_HANG, không tạo TAI_KHOAN_ADMIN, chưa chạy.
- Đăng nhập dùng hash trong GUI/admin/Security/AdminLoginService.cs; có thể override Admin:Username/Admin:PasswordHash qua User Secrets. Không ghi mật khẩu thô hoặc chuỗi kết nối vào tài liệu/Git.
- Ngoài phần đã làm: User checkout thật, Excel/PDF, VAT/tham số, hoàn trả sau giao, báo cáo nâng cao, ảnh mới, gom project và máy sạch chưa nghiệm thu.

## Chạy

```powershell
dotnet run --project GUI/user/SmartShop.User.csproj --launch-profile http
```

HTTP mặc định http://localhost:5264; HTTPS profile dùng https://localhost:7040. Mở /admin/login. Kết nối đọc ConnectionStrings:SmartShop, fallback SMARTSHOP_DB_CONNECTION; UserSecretsId SmartShop.Admin.Local. Tài khoản/mật khẩu do quản trị viên giữ riêng.
