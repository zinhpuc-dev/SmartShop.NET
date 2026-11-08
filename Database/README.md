# Database SmartShop

Cập nhật 08/11/2026: Admin hiện dùng **MySQL** quan_ly_cua_hang_dien_tu, không dùng SQL Server trong kế hoạch ban đầu.

- QuanLyCuaHangDienTu_MySQL.sql: schema tham chiếu 19 bảng/190 dòng mẫu, có DROP TABLE; không chạy lại trên database hiện có.
- migrations/2026-03-Admin-Auth-Order-Warehouse.sql: chỉ thêm DON_HANG.MaKho/index/FK, chưa áp dụng và đang chờ duyệt.
- Script/ERD SQL Server cũ nếu có là tài liệu lịch sử, không tự chạy để thay MySQL.
- Kết nối qua User Secrets của SmartShop.User, không ghi tài khoản/mật khẩu trong source hoặc tài liệu.
- Một Admin cục bộ dùng mật khẩu băm, không cần tạo TAI_KHOAN_ADMIN.

Đã đọc 10 đơn, 10 hóa đơn, 10 phiếu nhập cùng chi tiết, tổng tồn 485. Không chạy lệnh ghi trong đợt kiểm tra 08/11/2026. ERD, dữ liệu đủ 4 quý, tham số/VAT và thao tác ghi còn cần nghiệm thu riêng. Giữ phân công dữ liệu cho Vinh.
