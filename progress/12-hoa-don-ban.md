# Hóa đơn bán hàng (main-sub), trừ tồn

**Người phụ trách:** Phúc  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Bảo (mẫu ListView) |
| Yêu cầu / Use Case | YC3, YC6, UC08, UC09, UC10, UC12, UC13, UC14, UC22, UC23 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/hoadon` |
| Mục báo cáo | III (Phúc) |
| Ảnh form | `docs/images/12-hoa-don-ban/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Đã sửa lỗi sửa List trong foreach ở DAO đơn/hóa đơn. Đã đọc 10 đơn/10 chi tiết và 10 hóa đơn/10 chi tiết. Có mã lập hóa đơn từ đơn đã giao/hoàn thành, xác nhận thu tiền. Trừ tồn ở bước hoàn tất đơn, không phải lập hóa đơn; cần migration MaKho đã duyệt. Chưa thử ghi, VAT/giảm giá/hoàn trả sau giao chưa đầy đủ.

### Mốc triển khai và kiểm chứng

- [x] Đọc được 10 đơn và 10 hóa đơn kèm chi tiết từ MySQL.
- [x] Có mã lập/thu tiền hóa đơn và transaction chuyển trạng thái/trừ tồn.
- [ ] Duyệt migration; thử ghi/rollback/trừ tồn lặp và hoàn trả trên database riêng.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Form Hóa đơn main-sub: click một hóa đơn hiện các dòng chi tiết
- [ ] Giỏ hàng gộp vào các dòng chi tiết; tính thành tiền, tổng tiền, giảm giá, VAT
- [ ] Lưu hóa đơn và trừ tồn trong một transaction
- [ ] Phương thức thanh toán khi nhận hàng (COD)
- [ ] Lịch sử đơn hàng, chi tiết đơn hàng, tra cứu danh sách hóa đơn
- [ ] Cập nhật trạng thái đơn theo quy trình đã thống nhất
- [ ] Hủy đơn: hoàn tồn đúng một lần, kiểm tra trạng thái được hủy
- [ ] Field không nhập tay: mã tự sinh, thành tiền
- [ ] Chụp ảnh form, ghi công thức tính cho báo cáo
- [ ] Đã có ảnh form trong `docs/images/12-hoa-don-ban/`
- [ ] Đã viết phần báo cáo tương ứng (III (Phúc))

## Tiêu chí nghiệm thu

Click 1 hóa đơn hiện đúng chi tiết; bán thì tồn giảm; hủy thì hoàn tồn đúng một lần

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
