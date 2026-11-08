# III. Các chức năng chính — Bảo

Cập nhật 08/11/2026 theo mã nguồn và kiểm chứng đã thực hiện. Không thay đổi phân công thành viên. Các thao tác ghi MySQL chưa được nghiệm thu; không xem build thành công là hoàn tất toàn bộ nghiệp vụ.

## Sản phẩm và loại sản phẩm

1. **Giới thiệu:** Quản lý sản phẩm, tìm kiếm, lọc và ngừng hoạt động.
2. **Giao diện:** Mục Sản phẩm tại /admin. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Giá trị tiền được kiểm tra độ chính xác hai chữ số thập phân.
4. **Đầu vào:** Thông tin SAN_PHAM và mã loại hợp lệ.
5. **Đầu ra:** Danh sách sản phẩm và phản hồi nghiệp vụ.
6. **Xử lý:** Đã đọc 10 sản phẩm qua HTTP. Schema LOAI_SAN_PHAM hiện không có cột tên loại; không tự tạo cột hoặc giả lập tên. Chưa kiểm chứng ghi CRUD, quản lý ảnh/đơn vị tính hoặc toàn bộ yêu cầu nâng cao.

## Khuyến mãi và voucher

Đã có GUI/BUS/DAO MySQL, chuẩn hóa loại giảm giá, kiểm tra ngày và số lượng đã dùng. Đã đọc 10 voucher; thao tác ghi và đổi voucher đã sử dụng chưa nghiệm thu trên MySQL.

## Xuất nhập Excel và report/PDF

Chưa xác nhận triển khai, xuất file hoặc nghiệm thu. Không có file báo cáo DOC/PDF cuối cùng được tạo trong đợt này.
