# Cơ sở dữ liệu, ERD, dữ liệu mẫu

**Người phụ trách:** Vinh  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Phúc (chốt schema) |
| Yêu cầu / Use Case | YC5 (UNIQUE), hỗ trợ mọi chức năng |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/csdl` |
| Mục báo cáo | II |
| Ảnh form | `docs/images/02-co-so-du-lieu/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Hiện dùng MySQL quan_ly_cua_hang_dien_tu, schema 19 bảng/190 dòng mẫu; không phải mô hình SQL Server cũ. Migration DON_HANG.MaKho chưa chạy; chưa xác nhận ERD hoặc dữ liệu đủ 4 quý.

### Mốc triển khai và kiểm chứng

- [x] Đọc được đơn/hóa đơn/phiếu nhập có JOIN theo schema MySQL.
- [x] Giữ nguyên dữ liệu hiện có, không chạy SQL ghi hoặc schema gốc.
- [ ] Duyệt migration kho; hoàn tất ERD và dữ liệu theo yêu cầu nghiệm thu.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Vẽ ERD
- [ ] Script tạo bảng: NhanVien, KhachHang, NhaCungCap, SanPham, DonVi, HoaDon, CTHD, PhieuNhap, CTPN, ThamSo, TaiKhoan
- [ ] Khóa chính, khóa ngoại, UNIQUE, CHECK; cột tiếng Việt dùng nvarchar
- [ ] Dữ liệu mẫu đủ 4 quý, nhiều nhân viên, khách hàng, sản phẩm
- [ ] Chốt schema với Phúc trước khi các bạn khác viết DAO
- [ ] Chụp ERD rõ nét cho báo cáo mục II
- [ ] Đã có ảnh form trong `docs/images/02-co-so-du-lieu/`
- [ ] Đã viết phần báo cáo tương ứng (II)

## Tiêu chí nghiệm thu

Chạy script trên máy sạch tạo đủ bảng và dữ liệu mẫu; ERD rõ nét

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
