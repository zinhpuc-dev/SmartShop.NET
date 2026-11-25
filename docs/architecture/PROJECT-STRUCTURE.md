# Cấu trúc nhánh OOAD

Nhánh này có hai tầng thư mục với mục đích khác nhau:

- Các thư mục ở gốc (`Database/`, `docs/`, `src/SmartShop.UI/`) là nền tảng dùng chung của repository.
- `S4_Nhom05/` là gói chương trình và tài liệu dành riêng cho bài nộp OOAD.

```text
SmartShop.NET/
├── Database/
│   ├── scripts/                 # SQL dùng chung
│   └── diagrams/                # Sơ đồ cơ sở dữ liệu dùng chung
├── docs/
│   ├── architecture/            # Tài liệu kiến trúc, nhánh và cấu trúc
│   └── requirements/            # Yêu cầu chung
├── src/SmartShop.UI/            # Khung giao diện dùng chung
└── S4_Nhom05/                   # Gói nộp OOAD
    ├── BaoCao/                  # Báo cáo DOCX/PDF
    └── ChuongTrinh/
        ├── Database/            # SQL đi kèm gói nộp
        └── SourceCode/
            ├── src/
            │   ├── SmartShop.Presentation/   # Boundary: màn hình và tương tác
            │   ├── SmartShop.Application/    # Control: điều phối ca sử dụng
            │   ├── SmartShop.Domain/         # Entity, quy tắc và khái niệm nghiệp vụ
            │   └── SmartShop.Infrastructure/ # Lưu trữ và tích hợp bên ngoài
            └── tests/            # Kiểm thử cho từng tầng
```

`Database/src` không thuộc cấu trúc này; mã C# không đặt trong thư mục cơ sở dữ liệu. `SmartShop.Shared` cũng đã được bỏ vì hiện chưa có thành phần dùng chung nào cần thư viện riêng. Tệp `.gitkeep` chỉ giữ lại thư mục rỗng để Git hiển thị; có thể xóa khi thư mục đã có tệp thật.

Các tệp `.sln`, `.csproj`, SQL và báo cáo chưa được tạo vì hiện chưa có nội dung tương ứng. README trong `SourceCode/` mô tả vai trò các tầng OOAD.
