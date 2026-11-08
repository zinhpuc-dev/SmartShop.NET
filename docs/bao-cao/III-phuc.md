# III. Các chức năng chính — Phúc

Cập nhật 08/11/2026 theo mã nguồn và kiểm chứng đã thực hiện. Không thay đổi phân công thành viên. Các thao tác ghi MySQL chưa được nghiệm thu; không xem build thành công là hoàn tất toàn bộ nghiệp vụ.

## Đăng nhập Admin

1. **Giới thiệu:** Một tài khoản Admin cục bộ; không cung cấp đăng ký hoặc quản lý nhiều tài khoản.
2. **Giao diện:** /admin/login và các trang /admin. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Mật khẩu được xác minh bằng PasswordHasher; không lưu mật khẩu rõ trong mã nguồn.
4. **Đầu vào:** Tên đăng nhập và mật khẩu; cấu hình thay thế bằng Admin:Username và Admin:PasswordHash.
5. **Đầu ra:** Cookie phiên và quyền AdminOnly; đăng xuất hủy phiên.
6. **Xử lý:** Có CSRF cho biểu mẫu, giới hạn số lần đăng nhập và phiên 8 giờ. Đã kiểm tra chặn truy cập chưa đăng nhập, sai mật khẩu, thiếu CSRF, đăng nhập đúng và đăng xuất. Không cần tạo bảng tài khoản trong MySQL; không đưa thông tin đăng nhập vào báo cáo.

## Đơn hàng và hóa đơn

1. **Giới thiệu:** Xem đơn/chi tiết, cập nhật trạng thái, tạo hóa đơn và ghi nhận thanh toán.
2. **Giao diện:** Các mục Đơn hàng/Thống kê trên /admin. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Tổng tiền đối chiếu tổng chi tiết; doanh thu cộng hóa đơn đã thanh toán theo khoảng ngày.
4. **Đầu vào:** Đơn hàng, kho xuất, trạng thái và yêu cầu tạo/thanh toán hóa đơn.
5. **Đầu ra:** Danh sách đơn, hóa đơn, chi tiết và phản hồi nghiệp vụ.
6. **Xử lý:** Mã nguồn dùng transaction và kiểm tra trạng thái; trừ kho gắn với hoàn tất giao hàng, không trừ lại khi thanh toán hóa đơn. Đã đọc 10 đơn/10 chi tiết và 10 hóa đơn/10 chi tiết. Chưa kiểm chứng tạo hóa đơn, ghi thanh toán, trừ kho, rollback hay cạnh tranh đồng thời. DON_HANG.MaKho cần migration riêng trước các chuyển trạng thái ảnh hưởng tồn kho; migration chưa chạy. Hoàn tiền/VAT và quy tắc mở rộng chưa được nghiệm thu.
