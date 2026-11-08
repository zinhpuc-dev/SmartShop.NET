# Ghép giao diện web mobile vào develop

**Người phụ trách:** Triết
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

## Cập nhật hiện tại — 08/11/2026

Phần ghép GUI ngày 03/11 là lịch sử. Hiện Admin dùng MySQL và đăng nhập riêng; User vẫn đăng ký DemoShopDao, không kết luận checkout lưu MySQL. Solution còn nhiều project; chưa gom toàn bộ vì phạm vi mới chỉ Admin.

### Mốc triển khai và kiểm chứng

- [x] Biên dịch solution hiện tại và đọc trang Admin qua HTTP.
- [x] Giữ nguyên nghiệp vụ User và ghi rõ khác biệt với giai đoạn demo cũ.
- [ ] Nghiệm thu tương tác desktop/mobile và chốt riêng việc gom project.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [x] Gộp giao diện đã làm vào develop, giữ nguyên các file đã pull từ origin/develop.
- [x] Đặt giao diện khách hàng trong User và quản trị trong Admin.
- [x] Đặt mã DTO–BUS–DAO demo trong Database/src.
- [x] Giữ state chung khi điều hướng User/Admin, khai báo route /admin từ Razor assembly.
- [x] Build thành công và qua 14 kiểm tra nghiệp vụ.
- [x] Kiểm tra trình duyệt sau khi chuyển sườn: User mở được, thêm vào giỏ, Admin dùng cùng state và truy cập trực tiếp /admin thành công.
- [x] Chuẩn bị gộp trên develop và chỉ định đích push HEAD:develop theo yêu cầu.

## Ghi chú

Nguồn giao diện: feature/web-mobile-gui (d26c183). Sườn remote: da1d503. Các thay đổi so với source remote chỉ là file mới, không sửa hoặc xóa file gốc. Tài liệu chạy tại docs/KIEN_TRUC_WEB_MOBILE.md. Không cập nhật TONG_QUAN.md hoặc checklist của thành viên khác theo yêu cầu giữ nguyên file đã pull.

Build: thành công, 0 lỗi; có cảnh báo NU1900 do không truy cập được dịch vụ kiểm tra gói NuGet. Lệnh build mở rộng cần mạng đã bị người dùng từ chối, không chạy lại.


## Nhật ký cập nhật

| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |
