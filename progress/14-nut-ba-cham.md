# Nút "..." tìm kiếm và điền form bằng biến static

**Người phụ trách:** Phúc  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Bảo (form tìm sản phẩm), Triết (form tìm KH, NV, NCC) |
| Yêu cầu / Use Case | YC4 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/nutbacham` |
| Mục báo cáo | III (Phúc) |
| Ảnh form | `docs/images/14-nut-ba-cham/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Có Customer/Employee/Supplier Picker/Selector dùng EventCallback theo phiên, không static chung người dùng. Hóa đơn/nhập hàng hiện dùng dropdown; chưa nghiệm thu toàn bộ popup theo mẫu desktop.

### Mốc triển khai và kiểm chứng

- [x] Có các component tìm/chọn hồ sơ Admin trả callback.
- [x] Có danh sách chọn dữ liệu thật trong hóa đơn/phiếu nhập.
- [ ] Nghiệm thu popup và khác biệt với cơ chế static trong đề bài.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Phúc làm mẫu cơ chế nút "..." với biến static
- [ ] Bảo: form tìm sản phẩm (popup)
- [ ] Triết: form tìm khách hàng, nhân viên, nhà cung cấp
- [ ] Gắn vào Hóa đơn bán và Phiếu nhập
- [ ] Cả nhóm hiểu cơ chế để trả lời kiểm tra giấy
- [ ] Đã có ảnh form trong `docs/images/14-nut-ba-cham/`
- [ ] Đã viết phần báo cáo tương ứng (III (Phúc))

## Tiêu chí nghiệm thu

Nút "..." tìm, chọn và điền đúng thông tin vào form ở hóa đơn và phiếu nhập

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
