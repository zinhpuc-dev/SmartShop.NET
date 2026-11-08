# Chuyển giao giữa các công cụ AI

Dùng khi hết hạn mức của một công cụ (Claude Code, Codex, Gemini CLI, Copilot) và phải chuyển sang công cụ khác. Công cụ mới không nhớ cuộc trò chuyện cũ, nên mọi thứ cần ghi ở đây.

## Trước khi dừng
1. Commit những gì đang chạy được (hoặc `git stash`).
2. Điền mục bên dưới.

## Mẫu ghi

| Mục | Nội dung |
|---|---|
| Ngày giờ | |
| Người | |
| Chức năng (file trong progress/) | |
| Nhánh Git | |
| Công cụ vừa dừng | |
| Đã làm xong | |
| Đang làm dở (file nào, dòng nào) | |
| Việc tiếp theo | |
| Lỗi đang gặp | |

## Câu mở đầu cho công cụ mới
> Đọc AGENTS.md, progress/AI_HANDOFF.md và progress/<chức năng>.md. Chạy `git status` và `git diff` để xem phần đang làm dở. Tiếp tục từ mục "Việc tiếp theo". Chỉ sửa file thuộc chức năng này.

## Bàn giao hiện tại — 08/11/2026

- Chỉ chỉnh Admin; không sửa User, không dùng Computer Use. Sau lúc tạm không test, người dùng đã cho phép kiểm tra lại.
- Source thực tế: C:\Users\Admin\source\repos\SmartShop.NET. Không nhầm với thư mục chat hay script tạm ngoài repository.
- .NET 10 Razor/Blazor + MySQL quan_ly_cua_hang_dien_tu; vẫn nhiều project, chưa gom toàn ứng dụng.
- Đã khôi phục một Admin cục bộ bằng PasswordHasher/cookie, không cần bảng tài khoản; không đưa mật khẩu vào tài liệu.
- Đã sửa foreach/List của đơn/hóa đơn và loaded của kho; thêm lập/thu tiền hóa đơn, chi tiết phiếu, tồn theo kho, lọc báo cáo và validation voucher.
- Build solution: 0 lỗi, 0 cảnh báo. 19 smoke checks User còn lại pass (không phải test Admin). HTTP đăng nhập đúng/sai, thiếu CSRF, chặn ẩn danh và đăng xuất đều đạt. DAO chỉ đọc MySQL: 10 đơn + 10 chi tiết, 10 hóa đơn + 10 chi tiết, 10 phiếu nhập + 10 chi tiết, 10 dòng tồn kho tổng 485; 10 chương trình/10 voucher. Doanh thu hóa đơn đã thanh toán năm 2026: 114.100.000 đồng trên dữ liệu mẫu tại thời điểm kiểm tra.
- Chưa kiểm thử ghi CRUD, lập/thu tiền hóa đơn, cộng/trừ tồn, rollback hoặc concurrency trên database riêng. Chưa áp dụng migration DON_HANG.MaKho. Chưa nghiệm thu toàn bộ đồ án, UI tương tác hoặc máy sạch. Không coi build/HTTP 200/dữ liệu giả là bằng chứng nghiệp vụ ghi MySQL.
- Test Admin đã dọn; giữ 19 checks User và DemoShopDao đang được User dùng. Không xóa mã demo còn phục vụ User.
- Migration MaKho phải được duyệt trước; không chạy schema gốc chứa DROP TABLE hoặc tự thao tác database hiện có.
- Việc tiếp theo: người dùng nghiệm thu UI; nếu được phép thì kiểm thử ghi/rollback trên database riêng và trình script MaKho để duyệt. Không tự đánh dấu toàn bộ Admin/đồ án xong.
- Báo cáo: ADMIN_REPORT_CURRENT.md. Các file progress số và nháp báo cáo có sẵn đã được đồng bộ theo phần thực tế.
- Không commit/push trong đợt này; không thay đổi gói hoặc tính phí Copilot.

## Lịch sử chuyển giao
(Thêm mục mới ở trên cùng)
