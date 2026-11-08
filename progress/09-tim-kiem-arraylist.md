# Tìm kiếm trên ArrayList (BUS)

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Phúc (khung BUS) |
| Yêu cầu / Use Case | YC2, UC06 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/timkiem` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/09-tim-kiem-arraylist/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Admin hiện dùng List<T>/SearchText để tìm không dấu trên danh sách BUS đã nạp; không được ghi là ArrayList. Trang sản phẩm/đơn có lọc danh sách. Yêu cầu ArrayList đúng mẫu chưa nghiệm thu.

### Mốc triển khai và kiểm chứng

- [x] Có tìm kiếm trên danh sách đã nạp và chuẩn hóa tiếng Việt.
- [x] Có lọc mã/tên sản phẩm và mã/tên khách/trạng thái đơn hàng.
- [ ] Đối chiếu yêu cầu ArrayList/biểu thức tìm kiếm với giảng viên.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Nạp danh sách vào ArrayList trong BUS
- [ ] Viết hàm tìm kiếm trong BUS, không query lại CSDL
- [ ] Áp dụng cho Sản phẩm, Khách hàng, Nhân viên, Nhà cung cấp
- [ ] Chuẩn bị giải thích cho kiểm tra giấy 10 phút
- [ ] Đã có ảnh form trong `docs/images/09-tim-kiem-arraylist/`
- [ ] Đã viết phần báo cáo tương ứng (III (Triết))

## Tiêu chí nghiệm thu

Tìm kiếm chạy trên ArrayList, code nằm ở BUS

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
