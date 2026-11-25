# Thống kê: thu, chi, lợi nhuận, ma trận quý, bộ lọc

**Người phụ trách:** Vinh  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 12/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Triết (giao diện lọc) |
| Yêu cầu / Use Case | YC10a, YC10b, YC10c, UC27, UC28, UC29 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Vinh) |
| Ảnh form | `docs/images/16-thong-ke/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 12/11/2026

Triết bổ sung giao diện lọc thời gian thống kê theo khoảng ngày, tháng, quý và năm. Component dùng lại BoLocThoiGian nhận bộ lọc đã áp dụng và trả lựa chọn bằng EventCallback; dữ liệu nhập sai không thay kết quả đang xem. Bộ lọc được gắn vào trang thống kê Admin, chỉ áp dụng khi bấm Xem báo cáo.

BoLocThoiGianThongKe và KhoangThoiGianThongKe nằm ở DTO; XuLyKyThongKe và overload DonHangBUS.ThongKeBanHang xử lý kiểm tra/chuyển kỳ tại BUS. DAO và công thức doanh thu hiện có được giữ nguyên: chỉ cộng HOA_DON.TrangThai=Đã thanh toán theo NgayLapHoaDon. Cận trên là đầu ngày kế tiếp để lấy đủ ngày cuối kỳ; hỗ trợ năm nhuận và chuyển quý/năm, chặn năm ngoài miền 1000–9998 trước khi gọi DAO.

Đợt kiểm tra ngày 08/11/2026 đã đọc được 10 hóa đơn và doanh thu 114.100.000 đồng trên dữ liệu mẫu năm 2026; không chạy lại đối chiếu MySQL trong đợt bổ sung bộ lọc này. Chưa có chi phí/lợi nhuận, ma trận quý hay lọc tổ hợp KH/NV/SP đầy đủ.

### Mốc triển khai và kiểm chứng

- [x] Có mã báo cáo ngày và công thức hóa đơn đã thanh toán.
- [x] Đã đọc thực tế doanh thu mẫu năm 2026: 114.100.000 đồng (đợt 08/11/2026).
- [x] Triển khai component lọc khoảng ngày/tháng/quý/năm, nối với BUS và DAO hiện có; giữ nguyên sườn GUI/BUS/DAO/DTO.
- [x] Build solution: 0 lỗi, 0 cảnh báo; 98 smoke checks đạt, gồm 20 checks mới về năm nhuận, cuối tháng/quý/năm, cận ngày và chặn bộ lọc sai trước DAO bằng repository giả lập.
- [ ] Nghiệm thu thao tác bộ lọc trên trình duyệt và đối chiếu từng kỳ với MySQL. Browser Use đã từ chối quyền mở địa chỉ cục bộ trong đợt này; chưa có ảnh form mới.
- [ ] Hoàn thiện chi/lợi nhuận, ma trận quý và tổ hợp theo đề bài.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Tổng thu, tổng chi, lợi nhuận: từ ngày đến ngày; theo tháng, quý, năm
- [ ] Ba ma trận theo quý: NV x Quý, KH x Quý, SP x Quý
- [ ] Tổng tiền hóa đơn theo KH/NV/SP, theo ngày/tháng/quý/năm, từ ngày đến ngày
- [ ] Tổ hợp: KH + SP; NV + SP + năm
- [ ] Giao diện lọc đầy đủ (Triết): đã triển khai lọc thời gian; còn KH/NV/SP, tổ hợp và nghiệm thu thao tác
- [ ] Đối chiếu số liệu với dữ liệu gốc
- [ ] Chốt công thức lợi nhuận và loại hóa đơn tính doanh thu với giảng viên
- [ ] Đã có ảnh form trong `docs/images/16-thong-ke/`
- [ ] Đã viết phần báo cáo tương ứng (III (Vinh))

## Tiêu chí nghiệm thu

Thống kê khớp dữ liệu gốc; đủ 3 ma trận và các bộ lọc

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 12/11/2026 | Triết | Bổ sung BoLocThoiGian dùng lại; DTO bộ lọc và BUS chuyển khoảng ngày/tháng/quý/năm; nối báo cáo Admin, giữ cách tính doanh thu hiện có | Codex | Build 0 lỗi/0 cảnh báo; 98 checks đạt, gồm 20 checks kỳ thống kê dùng DAO giả lập. Chưa nghiệm thu trình duyệt/đối chiếu MySQL; còn lọc tổ hợp, chi/lợi nhuận và ma trận quý |
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

- Cần kiểm tra trực tiếp thao tác bộ lọc và đối chiếu các kỳ với dữ liệu gốc; chưa coi build và checks DAO giả lập là nghiệm thu giao diện hoặc số liệu MySQL.
- Phối hợp Vinh hoàn thiện lọc KH/NV/SP, lọc tổ hợp, chi/lợi nhuận và ba ma trận quý; bộ lọc thời gian không thay thế các yêu cầu này.
