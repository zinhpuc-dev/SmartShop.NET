# Quản lý Nhà cung cấp

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 12/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Vinh (UNIQUE) |
| Yêu cầu / Use Case | Danh mục NCC |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/07-nha-cung-cap/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 12/11/2026

NhaCungCapBUS → INhaCungCapDAO/NhaCungCapDAO → NHA_CUNG_CAP. Đã gắn OChonNhaCungCap và TimChonNhaCungCap vào form tạo phiếu nhập tại `/admin/inventory`, thay dropdown nhà cung cấp.

### Mốc triển khai và kiểm chứng

- [x] Đã đọc được 10 nhà cung cấp MySQL ở đợt kiểm tra trước.
- [x] Có CRUD/tìm kiếm và tích hợp popup chọn nhà cung cấp vào phiếu nhập ở mức code.
- [x] Callback điền SupplierCode và thông tin chỉ đọc; lưu phiếu thành công sẽ xóa lựa chọn cho phiếu mới. Có khóa nút khi tải dữ liệu lỗi và thông báo khi không đọc được hồ sơ đã chọn.
- [x] Build solution: 0 lỗi, 0 cảnh báo; 78 smoke checks hiện có đạt. Các checks này không nghiệm thu tương tác popup hay ghi phiếu MySQL.
- [ ] Kiểm thử thao tác popup trên trình duyệt, ghi/xóa có liên kết và chụp ảnh mới. Browser Use từ chối quyền mở địa chỉ cục bộ trong đợt này.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [x] CRUD Nhà cung cấp theo GUI-BUS-DAO (đã có DAO MySQL; thao tác ghi chưa nghiệm thu)
- [x] Kiểm tra trùng mã, field bắt buộc
- [x] Form tìm nhà cung cấp cho nút "..." ở phiếu nhập (đã gắn component và callback; còn nghiệm thu thao tác trực tiếp)
- [ ] Chụp ảnh form cho báo cáo
- [ ] Đã có ảnh form trong `docs/images/07-nha-cung-cap/`
- [ ] Đã viết phần báo cáo tương ứng (III (Triết))

### Phạm vi trước 08/11/2026 (lịch sử)

- Trang `/admin/suppliers` có trong menu Admin. Hỗ trợ thêm, sửa, xóa có xác nhận và tìm theo mã, tên, điện thoại, email hoặc địa chỉ.
- GUI dùng lại DataTable, ValidatedForm, FormField và ModalDialog. SupplierSelector và SupplierPicker trả nhà cung cấp đã chọn qua callback để có thể gắn vào phiếu nhập.
- BUS giữ ArrayList, tìm không dấu bằng SearchText, kiểm tra dữ liệu bắt buộc và mã trùng không phân biệt hoa/thường. Mã và ngày tạo được giữ nguyên khi sửa.
- DAO demo lưu trong bộ nhớ theo phiên qua ISupplierRepository. BUS chỉ cập nhật danh sách sau khi DAO lưu thành công.
- Build thành công với 0 lỗi, 0 cảnh báo. Toàn bộ 84 kiểm tra đạt, gồm 7 kiểm tra mới cho nhà cung cấp. Đã xác nhận trang Nhà cung cấp và các trang Admin liên quan trả HTTP 200.

## Tiêu chí nghiệm thu

Thêm, sửa, xóa, tìm được; không trùng mã

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 12/11/2026 | Triết | Gắn popup tìm/chọn nhà cung cấp vào phiếu nhập; khóa nút khi tải lỗi, hiển thị thông tin chỉ đọc và bổ sung xử lý lỗi chọn | Codex | Build 0 lỗi/0 cảnh báo, 78 checks hiện có đạt. Chưa nghiệm thu UI/ghi MySQL; chưa có ảnh mới |
| 04/11/2026 | Triết | Triển khai bản demo phần 07 với CRUD, tìm kiếm trên ArrayList, kiểm tra dữ liệu và popup chọn nhà cung cấp | Codex | Đang làm, 2/6 mục. Popup đã có trong Admin, chờ gắn vào phiếu nhập |

| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc lịch sử (đối chiếu cập nhật phía trên)

- Phối hợp Vinh để tích hợp SQL/MyConnection, ràng buộc UNIQUE và quan hệ với phiếu nhập. Hiện chưa lưu dữ liệu qua phiên và chưa kiểm tra được điều kiện xóa nhà cung cấp đã có phiếu nhập.
- Gắn popup chọn nhà cung cấp khi chức năng phiếu nhập sẵn sàng.
- Còn chụp ảnh form và viết phần báo cáo III.
