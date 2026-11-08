# Phiếu nhập hàng (main-sub), cộng tồn, điều chỉnh kho

**Người phụ trách:** Vinh  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Bảo (mẫu ListView) |
| Yêu cầu / Use Case | YC3, YC6, UC25, UC26 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/phieunhap` |
| Mục báo cáo | III (Vinh) |
| Ảnh form | `docs/images/13-phieu-nhap/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Đã sửa cờ loaded, thêm xem tồn theo kho và chi tiết phiếu. WarehouseService/WarehouseDAO có tạo phiếu/chi tiết/cộng tồn cùng transaction. Đã đọc 10 phiếu/10 chi tiết; tổng tồn 485. Chưa có nhật ký điều chỉnh kho riêng hoặc kiểm thử ghi/rollback.

### Mốc triển khai và kiểm chứng

- [x] Đọc 10 phiếu nhập và 10 dòng chi tiết.
- [x] Đọc 10 dòng tồn kho, tổng 485; có mã ghi transaction.
- [ ] Kiểm thử cộng tồn/rollback và nghiệp vụ điều chỉnh kho trên database riêng.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Form Phiếu nhập main-sub: click một phiếu hiện các dòng chi tiết
- [ ] Lưu phiếu và tăng tồn trong một transaction
- [ ] Điều chỉnh tồn kho, có lịch sử biến động kho
- [ ] Field không nhập tay: mã tự sinh, thành tiền
- [ ] Chụp ảnh form, ghi công thức cho báo cáo
- [ ] Đã có ảnh form trong `docs/images/13-phieu-nhap/`
- [ ] Đã viết phần báo cáo tương ứng (III (Vinh))

## Tiêu chí nghiệm thu

Nhập thì tồn tăng đúng; có lịch sử biến động kho

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
