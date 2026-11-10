# Tìm kiếm trên ArrayList (BUS)

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 10/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Phúc (khung BUS) |
| Yêu cầu / Use Case | YC2, UC06 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` (local, chưa commit/push) |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/09-tim-kiem-arraylist/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 10/11/2026

Đã triển khai tìm kiếm cơ bản trên ArrayList tại BUS cho bốn danh mục Admin gồm Sản phẩm, Khách hàng, Nhân viên và Nhà cung cấp. Trạng thái Đang làm, hoàn thành 3/6 mục trong checklist chính.

### Mốc triển khai và kiểm chứng

- CustomerBUS, EmployeeBUS và SupplierBUS nạp danh sách vào ArrayList khi cần, tìm trên bộ nhớ và chỉ đọc lại danh sách khi Reload. Giữ các hợp đồng BUS/DAO và cấu trúc thư mục của nhóm.
- ProductBUS giữ ArrayList, hỗ trợ tìm mã/tên không dấu kết hợp lọc loại sản phẩm. Ô tìm sản phẩm trong Admin gọi BUS. Search không gọi hàm tìm kiếm của DAO mỗi lần gõ.
- Dùng lại SearchText để chuẩn hóa tiếng Việt. Các danh mục liên hệ giữ các trường tìm kiếm sẵn có, nhân viên tìm thêm chức vụ.
- Danh sách BUS được cập nhật sau khi DAO ghi thành công. Sản phẩm tải lại sau thay đổi; kết quả tìm sản phẩm trả bản sao để chỉnh sửa ở nơi gọi không làm đổi cache.
- Build solution thành công, 0 lỗi và 0 cảnh báo. Toàn bộ 40 smoke checks đạt, gồm 19 kiểm tra User hiện có và 21 kiểm tra tìm kiếm mới trong `tools/SmartShop.SmokeTests/CatalogSearchChecks.cs`.
- Kiểm tra mới dùng repository giả lập, xác nhận tìm không dấu, số lần nạp danh sách, cập nhật cache sau CRUD, giữ cache khi xóa lỗi, lọc sản phẩm và tải lại dữ liệu. Chưa kiểm thử ghi MySQL thật hoặc đồng thời nhiều phiên.
- Đã cấu hình MySQL local và đăng nhập Admin, trang tổng quan đọc được 10 sản phẩm, 10 đơn hàng và tồn kho 485. Đây là bằng chứng ứng dụng đọc dữ liệu thật, chưa thay thế kiểm tra tương tác tìm kiếm của bốn danh mục trên MySQL.
- Còn chuẩn bị phần giải thích kiểm tra giấy, ảnh riêng phần 09 và báo cáo mục III. Biểu thức AND/OR/NOT và so sánh thuộc phần 10.

### Mốc trước — 08/11/2026

Tại thời điểm này, Admin dùng List<T>/SearchText và lọc danh sách tại một số trang. Yêu cầu ArrayList đúng mẫu chưa nghiệm thu. Mốc này được giữ làm lịch sử, đã được cập nhật bằng phần triển khai ngày 10/11/2026 ở trên.

## Việc cần làm

- [x] Nạp danh sách vào ArrayList trong BUS
- [x] Viết hàm tìm kiếm trong BUS, không query lại CSDL
- [x] Áp dụng cho Sản phẩm, Khách hàng, Nhân viên, Nhà cung cấp
- [ ] Chuẩn bị giải thích cho kiểm tra giấy 10 phút
- [ ] Đã có ảnh form trong `docs/images/09-tim-kiem-arraylist/`
- [ ] Đã viết phần báo cáo tương ứng (III (Triết))

## Tiêu chí nghiệm thu

Tìm kiếm chạy trên ArrayList, code nằm ở BUS

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 10/11/2026 | Triết | Chuyển tìm kiếm cơ bản của bốn danh mục Admin sang ArrayList tại BUS, nối tìm sản phẩm từ GUI vào BUS và thêm kiểm tra cache/tìm kiếm | Codex | Đang làm, 3/6 mục. Build đạt, 40 checks qua với repository giả lập. Local develop, chưa commit/push |
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

- Cần kiểm tra tương tác tìm kiếm từng danh mục với MySQL và đối chiếu/nhờ giảng viên nghiệm thu cách áp dụng ArrayList cho bản web.
- Còn chuẩn bị giải thích kiểm tra giấy 10 phút, chụp ảnh phần 09 và viết báo cáo mục III.
- Chưa kiểm thử ghi/rollback và cập nhật đồng thời trên MySQL. Cache của phiên khác cần tải lại để thấy thay đổi mới.
