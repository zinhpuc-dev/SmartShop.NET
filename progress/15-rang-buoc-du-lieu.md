# Ràng buộc dữ liệu: duy nhất, field bị khóa

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Vinh (UNIQUE trong CSDL), cả nhóm |
| Yêu cầu / Use Case | YC5, YC7 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/rangbuoc` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/15-rang-buoc-du-lieu/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

BUS kiểm tra đầu vào/mã/giá, DAO SQL có tham số/FK; sửa validation voucher theo chương trình/thời gian/số lượt dùng, chuẩn hóa PhanTram/SoTien. Chuyển trạng thái đơn và transaction có trong code; chưa thử ghi/concurrency.

### Mốc triển khai và kiểm chứng

- [x] Có validation và SQL tham số trong các module Admin.
- [x] Giao diện phân biệt tải/rỗng/lỗi, thông báo thân thiện.
- [ ] Kiểm thử ràng buộc ghi, rollback, dữ liệu trùng và concurrency.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Kiểm tra duy nhất: mã khách, sản phẩm, nhân viên...
- [ ] UNIQUE trong CSDL (Vinh) và kiểm tra ở BUS (Triết)
- [ ] Field không được nhập: mã tự sinh, thành tiền, tồn kho
- [ ] Thông báo lỗi rõ ràng cho người dùng
- [ ] Đã có ảnh form trong `docs/images/15-rang-buoc-du-lieu/`
- [ ] Đã viết phần báo cáo tương ứng (III (Triết))

## Tiêu chí nghiệm thu

Không trùng mã; field khóa không sửa được

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
