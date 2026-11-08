# Thống kê: thu, chi, lợi nhuận, ma trận quý, bộ lọc

**Người phụ trách:** Vinh  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Triết (giao diện lọc) |
| Yêu cầu / Use Case | YC10a, YC10b, YC10c, UC27, UC28, UC29 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/thongke` |
| Mục báo cáo | III (Vinh) |
| Ảnh form | `docs/images/16-thong-ke/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Có lọc khoảng ngày, chỉ cộng HOA_DON.TrangThai=Đã thanh toán theo NgayLapHoaDon. Kiểm tra năm 2026: 10 hóa đơn, 114.100.000 đồng trên dữ liệu mẫu. Chưa có chi phí/lợi nhuận, ma trận quý hoặc lọc tổ hợp đầy đủ.

### Mốc triển khai và kiểm chứng

- [x] Có mã báo cáo ngày và công thức hóa đơn đã thanh toán.
- [x] Đọc thực tế doanh thu mẫu năm 2026: 114.100.000 đồng.
- [ ] Hoàn thiện chi/lợi nhuận, ma trận quý và tổ hợp theo đề bài.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Tổng thu, tổng chi, lợi nhuận: từ ngày đến ngày; theo tháng, quý, năm
- [ ] Ba ma trận theo quý: NV x Quý, KH x Quý, SP x Quý
- [ ] Tổng tiền hóa đơn theo KH/NV/SP, theo ngày/tháng/quý/năm, từ ngày đến ngày
- [ ] Tổ hợp: KH + SP; NV + SP + năm
- [ ] Giao diện lọc (Triết)
- [ ] Đối chiếu số liệu với dữ liệu gốc
- [ ] Chốt công thức lợi nhuận và loại hóa đơn tính doanh thu với giảng viên
- [ ] Đã có ảnh form trong `docs/images/16-thong-ke/`
- [ ] Đã viết phần báo cáo tương ứng (III (Vinh))

## Tiêu chí nghiệm thu

Thống kê khớp dữ liệu gốc; đủ 3 ma trận và các bộ lọc

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
