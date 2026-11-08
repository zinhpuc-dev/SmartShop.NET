# Phần 06 — Quản lý nhân viên trên web

Cập nhật 08/11/2026 theo mã nguồn và kiểm chứng đã thực hiện. Không thay đổi phân công thành viên. Các thao tác ghi MySQL chưa được nghiệm thu; không xem build thành công là hoàn tất toàn bộ nghiệp vụ.

## Quản lý nhân viên

1. **Giới thiệu:** Danh sách, tìm kiếm và CRUD nhân viên.
2. **Giao diện:** Trang /admin/employees. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin NHAN_VIEN, bao gồm ChucVu; BUS kiểm tra dữ liệu.
5. **Đầu ra:** Danh sách và phản hồi thao tác.
6. **Xử lý:** GUI gọi BUS và DAO MySQL, tải lại danh sách thay vì giữ bộ nhớ đệm cũ. Đã đọc 10 hồ sơ qua HTTP; chưa nghiệm thu thao tác ghi. Chức vụ hồ sơ nhân viên không đồng nghĩa với phân quyền đăng nhập nhiều tài khoản.

## Tư liệu cũ

Ảnh và kết quả kiểm tra demo trước đây chỉ có giá trị lịch sử; cần cập nhật ảnh bản MySQL và nghiệm thu ghi dữ liệu riêng.
