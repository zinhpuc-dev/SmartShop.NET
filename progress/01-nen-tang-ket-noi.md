# Nền tảng 3 lớp, MyConnection, tiếng Việt

**Người phụ trách:** Phúc  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Cả nhóm |
| Yêu cầu / Use Case | YC1, YC15 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/nentang` |
| Mục báo cáo | IV |
| Ảnh form | `docs/images/01-nen-tang-ket-noi/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Admin dùng GUI → BUS → hợp đồng DTO → DAO MySQL; ConnectionStrings:SmartShop/User Secrets và fallback SMARTSHOP_DB_CONNECTION. Chưa có lớp MyConnection dùng chung đúng checklist gốc; chưa nghiệm thu máy sạch.

### Mốc triển khai và kiểm chứng

- [x] Cấu hình ngoài source và đọc dữ liệu tiếng Việt thật.
- [x] Biên dịch solution 0 lỗi, 0 cảnh báo.
- [ ] Chạy máy sạch, giải thích chéo và đối chiếu MyConnection với yêu cầu đồ án.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Tạo solution GUI-BUS-DAO (và DTO nếu tách), đẩy lên GitHub
- [ ] Quy ước branch (main, develop, feature/...) và quy ước commit
- [ ] Viết class MyConnection trong DAO: mở/đóng kết nối, hàm chạy query dùng chung, truy vấn có tham số
- [ ] Chuỗi kết nối để ở một chỗ duy nhất, chạy được trên máy khác
- [ ] Viết khung DAO/BUS mẫu (ví dụ Sản phẩm) để mọi người làm theo cùng kiểu
- [ ] Xử lý tiếng Việt: cột nvarchar, hiển thị đúng trên form (phối hợp Vinh)
- [ ] Họp giải thích chéo MyConnection, DAO, BUS cho cả nhóm (chuẩn bị kiểm tra giấy)
- [ ] Đã có ảnh form trong `docs/images/01-nen-tang-ket-noi/`
- [ ] Đã viết phần báo cáo tương ứng (IV)

## Tiêu chí nghiệm thu

Cấu trúc GUI-BUS-DAO rõ ràng, có MyConnection; tiếng Việt hiển thị đúng; clone về máy khác chạy được

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
