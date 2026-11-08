# II. Thiết kế CSDL

Phụ trách: Vinh. Cập nhật 08/11/2026.

Cơ sở dữ liệu hiện tại là MySQL quan_ly_cua_hang_dien_tu, không phải SQL Server. Kiểm tra trước đó ghi nhận 19 bảng và 190 dòng dữ liệu mẫu. Các nhóm Admin sử dụng KHACH_HANG, NHAN_VIEN, NHA_CUNG_CAP, SAN_PHAM, LOAI_SAN_PHAM, TON_KHO, DON_HANG và các bảng chi tiết/phiếu nhập/hóa đơn/khuyến mãi liên quan theo schema thực tế.

GUI gọi BUS để kiểm tra nghiệp vụ rồi DAO thực hiện truy vấn có tham số. Các luồng nhập hàng, hóa đơn và cập nhật tồn có transaction trong mã nguồn, nhưng chưa nghiệm thu ghi/rollback trên MySQL.

Database/QuanLyCuaHangDienTu_MySQL.sql chỉ là schema tham chiếu và chứa DROP TABLE: không chạy lại trên dữ liệu hiện tại. Migration bổ sung DON_HANG.MaKho, index và khóa ngoại chưa áp dụng. Đăng nhập Admin hiện xác thực cục bộ bằng hash, không cần bảng TAI_KHOAN_ADMIN. Không có thay đổi dữ liệu hoặc schema trong đợt kiểm tra.

ERD, ảnh quan hệ và dữ liệu mẫu đủ các quý theo yêu cầu báo cáo vẫn cần hoàn thiện; chưa có ERD mới được sinh ra.
