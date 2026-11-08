# Phần 05 — Quản lý khách hàng trên web

Cập nhật 08/11/2026 theo mã nguồn và kiểm chứng đã thực hiện. Không thay đổi phân công thành viên. Các thao tác ghi MySQL chưa được nghiệm thu; không xem build thành công là hoàn tất toàn bộ nghiệp vụ.

## Quản lý khách hàng

1. **Giới thiệu:** Danh sách, tìm kiếm và CRUD khách hàng qua GUI → BUS → DAO MySQL.
2. **Giao diện:** Trang /admin/customers. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin hồ sơ phù hợp KHACH_HANG; kiểm tra bắt buộc và định dạng tại BUS.
5. **Đầu ra:** Danh sách và thông báo thành công/lỗi phù hợp thao tác.
6. **Xử lý:** DAO đọc/ghi KHACH_HANG; BUS làm mới danh sách sau thao tác. Đã đọc HTTP thành công 10 hồ sơ, chưa thử thêm/sửa/xóa trên MySQL. Các mô tả khóa/mở hồ sơ trong bản demo cũ không phải cam kết về cột có trong schema hiện tại.

## Tư liệu cũ

Ảnh tại docs/images/05-khach-hang thuộc giai đoạn demo trước; cần chụp lại để xác nhận giao diện và trường dữ liệu hiện tại. Không dùng kết quả test demo cũ để kết luận CRUD MySQL đã đạt.
