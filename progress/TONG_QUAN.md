# Tổng quan tiến độ SmartShop

Cập nhật bằng lệnh (chạy ở gốc repo): `node tools/cap-nhat-tien-do.js`
Bảng bên dưới do script sinh ra từ các file trong `progress/`. Đừng sửa tay giữa hai dòng đánh dấu.

## Hiện trạng 08/11/2026

Admin đã có các luồng MySQL và đăng nhập cục bộ; đã build, kiểm tra HTTP và đọc DAO. Chưa nghiệm thu ghi CRUD, rollback, trừ kho hoặc giao diện tương tác đầy đủ; migration chưa áp dụng. User giữ nguyên. Tỷ lệ bên dưới đếm checklist kế hoạch và mốc bổ sung, không phải tỷ lệ sẵn sàng vận hành. Chi tiết: ADMIN_REPORT_CURRENT.md và AI_HANDOFF.md.

<!-- BEGIN:TIEN_DO -->
_Cập nhật tự động: 8/11/2026 13:43:52_

**Tổng: 60/215 việc (28%)**

| File | Chức năng | Phụ trách | Trạng thái | Việc | % |
|---|---|---|---|---|---|
| 01-nen-tang-ket-noi | Nền tảng 3 lớp, MyConnection, tiếng Việt | Phúc | Đang làm | 2/12 | 17% |
| 02-co-so-du-lieu | Cơ sở dữ liệu, ERD, dữ liệu mẫu | Vinh | Đang làm | 2/11 | 18% |
| 03-dang-nhap-phan-quyen | Đăng ký, đăng nhập, phân quyền | Phúc | Đang làm | 4/10 | 40% |
| 04-form-chinh | Form chính (menu trái, form con) | Triết | Đang làm | 5/9 | 56% |
| 05-khach-hang | Quản lý Khách hàng | Triết | Đang làm | 7/10 | 70% |
| 06-nhan-vien | Quản lý Nhân viên | Triết | Đang làm | 7/9 | 78% |
| 07-nha-cung-cap | Quản lý Nhà cung cấp | Triết | Đang làm | 4/9 | 44% |
| 08-san-pham-don-vi | Sản phẩm, Đơn vị, tồn kho | Bảo | Đang làm | 2/11 | 18% |
| 09-tim-kiem-arraylist | Tìm kiếm trên ArrayList (BUS) | Triết | Đang làm | 2/9 | 22% |
| 10-tim-kiem-nang-cao | Tìm kiếm nâng cao (and, or, not, so sánh) | Triết | Chưa làm | 0/7 | 0% |
| 11-bang-tham-so | Bảng tham số (VAT, ngưỡng tồn, tên cửa hàng) | Vinh | Chưa làm | 0/6 | 0% |
| 12-hoa-don-ban | Hóa đơn bán hàng (main-sub), trừ tồn | Phúc | Đang làm | 2/14 | 14% |
| 13-phieu-nhap | Phiếu nhập hàng (main-sub), cộng tồn, điều chỉnh kho | Vinh | Đang làm | 2/10 | 20% |
| 14-nut-ba-cham | Nút "..." tìm kiếm và điền form bằng biến static | Phúc | Đang làm | 2/10 | 20% |
| 15-rang-buoc-du-lieu | Ràng buộc dữ liệu: duy nhất, field bị khóa | Triết | Đang làm | 2/9 | 22% |
| 16-thong-ke | Thống kê: thu, chi, lợi nhuận, ma trận quý, bộ lọc | Vinh | Đang làm | 2/12 | 17% |
| 17-xuat-nhap-excel | Export/Import file xls | Bảo | Chưa làm | 0/6 | 0% |
| 18-report-pdf | Report PDF: hóa đơn và bảng thống kê | Bảo | Chưa làm | 0/6 | 0% |
| 19-mo-rong-uc15-19 | Mở rộng: FAQ/Chatbot, TTS, đa ngôn ngữ | Bảo | Chưa làm | 0/6 | 0% |
| 20-bao-cao | Báo cáo cuối kỳ | Phúc | Đang làm | 2/11 | 18% |
| 21-kiem-thu-nghiem-thu | Kiểm thử, nghiệm thu, chạy máy sạch | Phúc | Đang làm | 2/10 | 20% |
| 22-cho-xac-nhan | Chờ xác nhận: UC04, UC11, Web/Mobile/API | Phúc | Đang làm | 2/8 | 25% |
| 23-web-mobile-gui | Ghép giao diện web mobile vào develop | Triết | Đang làm | 9/10 | 90% |

| Thành viên | Việc | % |
|---|---|---|
| Phúc | 16/75 | 21% |
| Vinh | 6/39 | 15% |
| Triết | 36/72 | 50% |
| Bảo | 2/29 | 7% |
<!-- END:TIEN_DO -->

## Mốc thời gian (theo bảng phân công v1.0)

| Giai đoạn | Thời gian | Mốc nghiệm thu |
|---|---|---|
| 1. Khởi tạo | 21-23/10 | Cả nhóm chạy được solution, kết nối CSDL, thấy form chính |
| 2. Danh mục | 24-29/10 | Mọi danh mục thêm, sửa, xóa, tìm được |
| 3. Nghiệp vụ | 30/10-05/11 | Lập được hóa đơn và phiếu nhập, tồn kho đổi đúng |
| 4. Thống kê, báo cáo | 06-10/11 | Thống kê khớp dữ liệu, in được PDF |
| 5. Chốt | 11-15/11 | Bản đóng băng, không còn lỗi nghiêm trọng |
| 6. Báo cáo | 16/11 - tháng 12 | Báo cáo hoàn chỉnh, nộp trong tháng 12/2026 |
