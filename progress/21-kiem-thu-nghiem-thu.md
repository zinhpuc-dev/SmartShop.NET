# Kiểm thử, nghiệm thu, chạy máy sạch

**Người phụ trách:** Phúc  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Cả nhóm |
| Yêu cầu / Use Case | YC16 |
| Mức ưu tiên | 1 |
| Nhánh Git | `release/final` |
| Mục báo cáo | IV |
| Ảnh form | `docs/images/21-kiem-thu-nghiem-thu/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Build solution 0 lỗi/cảnh báo; 19 checks User còn lại pass. HTTP đăng nhập đúng/sai/CSRF/chặn ẩn danh/đăng xuất đạt. DAO chỉ đọc: 10 đơn, 10 hóa đơn, 10 phiếu cùng chi tiết, tồn 485. Chưa thử ghi/rollback/trừ tồn/concurrency hoặc máy sạch. Test Admin đã dọn theo yêu cầu, 19 checks User không chứng minh nghiệp vụ MySQL. Công cụ tạm ngoài repo đã xóa.

### Mốc triển khai và kiểm chứng

- [x] Build và 19 checks User pass.
- [x] Kiểm tra HTTP xác thực và DAO chỉ đọc MySQL đạt.
- [ ] Kiểm thử ghi trên database riêng, UI tương tác và nghiệm thu máy sạch.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Kiểm thử hồi quy sau khi tích hợp
- [ ] Chạy thử trên máy sạch, có dữ liệu mẫu và tài khoản demo
- [ ] Đi qua checklist nghiệm thu mục 10 của bảng phân công
- [ ] Ôn kiểm tra giấy 10 phút (tuần 9-10): viết 1-2 hàm
- [ ] Chuẩn bị demo chấm đồ án (tuần 11-12)
- [ ] Đã có ảnh form trong `docs/images/21-kiem-thu-nghiem-thu/`
- [ ] Đã viết phần báo cáo tương ứng (IV)

## Tiêu chí nghiệm thu

Bản đóng băng, không còn lỗi nghiêm trọng

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
