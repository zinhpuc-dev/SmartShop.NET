# III. Các chức năng chính — Triết

Cập nhật 08/11/2026 theo mã nguồn và kiểm chứng đã thực hiện. Không thay đổi phân công thành viên. Các thao tác ghi MySQL chưa được nghiệm thu; không xem build thành công là hoàn tất toàn bộ nghiệp vụ.

## Quản lý khách hàng

1. **Giới thiệu:** Danh sách, tìm kiếm và CRUD khách hàng qua GUI → BUS → DAO MySQL.
2. **Giao diện:** Trang /admin/customers. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin hồ sơ phù hợp KHACH_HANG; kiểm tra bắt buộc và định dạng tại BUS.
5. **Đầu ra:** Danh sách và thông báo thành công/lỗi phù hợp thao tác.
6. **Xử lý:** DAO đọc/ghi KHACH_HANG; BUS làm mới danh sách sau thao tác. Đã đọc HTTP thành công 10 hồ sơ, chưa thử thêm/sửa/xóa trên MySQL. Các mô tả khóa/mở hồ sơ trong bản demo cũ không phải cam kết về cột có trong schema hiện tại.

## Quản lý nhân viên

1. **Giới thiệu:** Danh sách, tìm kiếm và CRUD nhân viên.
2. **Giao diện:** Trang /admin/employees. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin NHAN_VIEN, bao gồm ChucVu; BUS kiểm tra dữ liệu.
5. **Đầu ra:** Danh sách và phản hồi thao tác.
6. **Xử lý:** GUI gọi BUS và DAO MySQL, tải lại danh sách thay vì giữ bộ nhớ đệm cũ. Đã đọc 10 hồ sơ qua HTTP; chưa nghiệm thu thao tác ghi. Chức vụ hồ sơ nhân viên không đồng nghĩa với phân quyền đăng nhập nhiều tài khoản.

## Quản lý nhà cung cấp

1. **Giới thiệu:** Quản lý hồ sơ nhà cung cấp.
2. **Giao diện:** Trang /admin/suppliers. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin phù hợp NHA_CUNG_CAP.
5. **Đầu ra:** Danh sách hồ sơ và thông báo thao tác.
6. **Xử lý:** Đã có DAO MySQL, BUS và giao diện; kiểm chứng HTTP đọc 10 hồ sơ. Chưa kiểm chứng ghi, xóa hồ sơ đang được tham chiếu hoặc xử lý đồng thời.

## Tìm kiếm và bố cục

Admin dùng bố cục riêng và danh sách List<T>; tìm/lọc trên từng màn hình. Không mô tả là WinForms hoặc ArrayList. Các popup tra cứu, tìm kiếm nâng cao và ảnh trong kế hoạch cũ cần đối chiếu lại trước khi đánh dấu hoàn tất.
