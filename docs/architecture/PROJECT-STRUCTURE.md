# Cấu trúc nền tảng chung

```text
SmartShop.NET/
├── src/
│   ├── SmartShop.UI/       # Giao diện chung; chia theo khu vực quản trị/người dùng
│   └── SmartShop.Shared/   # Thành phần dùng chung giữa giao diện và backend, nếu cần
├── Database/
│   ├── scripts/            # Script tạo/cập nhật cơ sở dữ liệu và dữ liệu mẫu
│   └── diagrams/           # Sơ đồ cơ sở dữ liệu dùng chung
├── docs/
│   ├── architecture/       # Quy ước kiến trúc và chiến lược nhánh
│   └── requirements/       # Yêu cầu sản phẩm dùng chung
├── tests/                  # Kiểm thử dùng chung
├── tools/                  # Công cụ hỗ trợ dự án
├── .github/                # Cấu hình cộng tác trên GitHub
└── README.md               # Giới thiệu ngắn về dự án
```

Các dự án .NET (`.sln`/`.slnx`, `.csproj`) sẽ được thêm khi thống nhất framework giao diện và phiên bản .NET. Chỉ đưa code vào `SmartShop.Shared` khi cả hai nhánh cần cùng một hợp đồng hoặc thành phần; không dùng thư mục này để ép hai backend theo cùng một kiến trúc.
