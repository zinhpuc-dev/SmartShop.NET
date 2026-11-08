# III. Các chức năng chính — Vinh

Cập nhật 08/11/2026 theo mã nguồn và kiểm chứng đã thực hiện. Không thay đổi phân công thành viên. Các thao tác ghi MySQL chưa được nghiệm thu; không xem build thành công là hoàn tất toàn bộ nghiệp vụ.

## Kho và phiếu nhập

1. **Giới thiệu:** Quản lý kho, xem tồn từng kho và chi tiết phiếu nhập.
2. **Giao diện:** /admin/inventory. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Thành tiền dòng = số lượng × đơn giá; tồn sau nhập = tồn trước + số lượng nhập.
4. **Đầu vào:** Kho, nhà cung cấp, sản phẩm, số lượng và đơn giá.
5. **Đầu ra:** Phiếu nhập, chi tiết và tồn kho.
6. **Xử lý:** Mã nguồn ghi đầu phiếu/chi tiết/tồn trong cùng transaction. Đã đọc 10 phiếu, 10 chi tiết và 10 dòng tồn, tổng 485 sản phẩm. Chưa thực hiện ghi phiếu nhập hoặc thử rollback trên MySQL.

## Thống kê doanh thu

1. **Giới thiệu:** Tổng hợp doanh thu theo khoảng ngày và chỉ số tổng quan.
2. **Giao diện:** Thẻ tổng quan và mục Thống kê tại /admin. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Doanh thu = tổng hóa đơn đã thanh toán trong khoảng thời gian chọn.
4. **Đầu vào:** Ngày bắt đầu và ngày kết thúc.
5. **Đầu ra:** Số hóa đơn, doanh thu và dữ liệu thống kê.
6. **Xử lý:** Kiểm tra đọc trực tiếp năm 2026 ghi nhận 10 hóa đơn đã thanh toán, tổng 114.100.000 đồng tại thời điểm kiểm tra. Đây là ảnh chụp dữ liệu lúc kiểm tra, không phải số cố định của chương trình. Báo cáo lợi nhuận, chi phí, ma trận quý hoặc tổng hợp nhóm theo kế hoạch chưa được xác nhận hoàn tất.

## Bảng tham số

Chưa triển khai/kiểm chứng đầy đủ màn hình tham số theo kế hoạch ban đầu; không đánh dấu hoàn thành.
