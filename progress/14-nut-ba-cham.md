# Nút "..." tìm kiếm và điền form bằng biến static

**Người phụ trách:** Phúc  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 12/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Bảo (form tìm sản phẩm), Triết (form tìm KH, NV, NCC) |
| Yêu cầu / Use Case | YC4 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Phúc) |
| Ảnh form | `docs/images/14-nut-ba-cham/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 12/11/2026

Phần phối hợp của Triết: có OChonKhachHang/TimChonKhachHang, OChonNhanVien/TimChonNhanVien và OChonNhaCungCap/TimChonNhaCungCap. Đã gắn popup nhân viên vào form lập hóa đơn tại `/admin/orders`, popup nhà cung cấp vào form tạo phiếu nhập tại `/admin/inventory`.

### Mốc triển khai và kiểm chứng

- [x] Component dùng lại, trả DTO bằng EventCallback theo phiên; thông tin được chọn hiển thị chỉ đọc. Giữ nguyên sườn GUI/BUS/DAO/DTO.
- [x] Mã nhân viên được chuyển vào HoaDonBUS.LapHoaDon; mã nhà cung cấp được điền vào DuLieuPhieuNhap.SupplierCode. Đóng/hủy popup không thay lựa chọn; callback chỉ chạy khi chọn.
- [x] Thêm Disabled cho ba ô chọn; khóa nút khi dữ liệu nghiệp vụ chưa tải được. Popup nhân viên/nhà cung cấp thông báo lỗi đọc hồ sơ và giữ popup để thử lại.
- [x] Build solution 0 lỗi/0 cảnh báo và 78 smoke checks hiện có đạt. Chưa có kiểm thử tự động riêng cho tương tác popup.
- [ ] Nghiệm thu thao tác trực tiếp, ghi hóa đơn/phiếu nhập và ảnh form. Browser Use từ chối quyền mở địa chỉ cục bộ trong đợt này.
- [ ] Phối hợp Phúc/Bảo hoàn thiện toàn bộ YC4, gồm popup sản phẩm. Bản web dùng callback theo phiên; cơ chế biến static đặc thù desktop còn cần đối chiếu với giảng viên.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Phúc làm mẫu cơ chế nút "..." với biến static
- [ ] Bảo: form tìm sản phẩm (popup)
- [x] Triết: form tìm khách hàng, nhân viên, nhà cung cấp (đã triển khai; còn nghiệm thu tương tác)
- [ ] Gắn đầy đủ vào Hóa đơn bán và Phiếu nhập (đã gắn nhân viên/NCC; còn phần sản phẩm phối hợp Bảo/Phúc)
- [ ] Cả nhóm hiểu cơ chế để trả lời kiểm tra giấy
- [ ] Đã có ảnh form trong `docs/images/14-nut-ba-cham/`
- [ ] Đã viết phần báo cáo tương ứng (III (Phúc))

## Tiêu chí nghiệm thu

Nút "..." tìm, chọn và điền đúng thông tin vào form ở hóa đơn và phiếu nhập

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 12/11/2026 | Triết | Tái sử dụng popup nhân viên cho lập hóa đơn, popup NCC cho phiếu nhập; nối callback với dữ liệu nghiệp vụ và bổ sung khóa nút/xử lý lỗi | Codex | Build 0 lỗi/0 cảnh báo, 78 checks hiện có đạt. Chưa nghiệm thu thao tác/ghi MySQL; không dùng static chung các phiên web |
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
