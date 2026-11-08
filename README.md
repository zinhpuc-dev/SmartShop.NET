# SmartShop

## Trạng thái thực tế — 08/11/2026

Mã hiện tại là .NET 10 Razor/Blazor và MySQL. Admin có đăng nhập/DAO thật và đã kiểm tra chỉ đọc; User vẫn dùng dữ liệu demo. Chưa nghiệm thu thao tác ghi, chưa chạy migration kho, chưa gom các project. Xem [báo cáo Admin](ADMIN_REPORT_CURRENT.md), [tiến độ](progress/TONG_QUAN.md), [hướng dẫn chạy](docs/bao-cao/IV-huong-dan-cai-dat.md).

Mô tả WinForms/SQL Server và cây thư mục dưới đây là kế hoạch ban đầu, giữ để đối chiếu.

## Kế hoạch ban đầu (lịch sử)

Đồ án học phần Lập trình C#: phần mềm quản lý bán hàng đồ điện tử. WinForms 3 lớp GUI-BUS-DAO, SQL Server.

## Cấu trúc thư mục

```
SmartShop/
├── GUI/ # WinForms (Forms/Main, Auth, DanhMuc, NghiepVu, ThongKe, TimKiem)
	├──admin/      # Form quản trị, thêm/sửa/xóa người dùng
	├──user/       # Form người dùng, thêm/sửa/xóa sản phẩm, hóa đơn
├── BUS/         # Nghiệp vụ, tìm kiếm ArrayList
	├──admin/     
	├──user/   
├── DAO/# Truy cập CSDL, MyConnection
	├──admin/     
	├──user/   
├── DTO/         # Các class thực thể
├── Database/              # Script SQL, dữ liệu mẫu, ERD
├── docs/
│   ├── word/              # Nơi lưu file .docx
│   ├── pdf/               # Yêu cầu của giảng viên, báo cáo PDF
│   ├── images/            # Ảnh form, mỗi chức năng một thư mục
│   ├── erd/               # Ảnh ERD
│   ├── bao-cao/           # Bản nháp từng mục báo cáo (.md)
│   ├── ai/                # Ghi chú dùng AI
│   └── KE_HOACH_CHI_TIET.md
├── progress/              # Báo cáo tiến độ từng chức năng (.md)
│   ├── TONG_QUAN.md       # Bảng tổng hợp, tự sinh
│   ├── AI_HANDOFF.md      # Chuyển giao khi đổi công cụ AI
│   ├── nhat-ky/           # Nhật ký họp hằng ngày
│   └── 01-... đến 22-...  # Mỗi chức năng một file
├── tools/
│   └── cap-nhat-tien-do.js
├── AGENTS.md              # Quy tắc chung cho mọi công cụ AI
└── .github/copilot-instructions.md
```

## Báo cáo tiến độ

1. Làm xong một việc nhỏ thì tick `- [x]` trong file `progress/<chức năng>.md`, thêm một dòng vào Nhật ký tiến độ, sửa dòng `Trạng thái`.
2. Chạy `node tools/cap-nhat-tien-do.js` để cập nhật `progress/TONG_QUAN.md`.
3. Commit file progress cùng với code của chức năng đó.
