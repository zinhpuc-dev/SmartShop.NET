# Mã nguồn OOAD — S4_Nhom05

## Các tầng

- `SmartShop.Presentation`: Boundary, nhận thao tác và hiển thị dữ liệu. Chia theo nghiệp vụ như Auth, Sản phẩm, Khách hàng, Hóa đơn.
- `SmartShop.Application`: Control, điều phối từng ca sử dụng; `DTOs` truyền dữ liệu giữa các tầng; `Interfaces` khai báo hợp đồng cần thiết.
- `SmartShop.Domain`: mô hình nghiệp vụ cốt lõi gồm Entity, ValueObjects, Enums, Rules và RepositoryContracts. Tầng này không phụ thuộc giao diện hay cơ sở dữ liệu.
- `SmartShop.Infrastructure`: hiện thực lưu trữ và dịch vụ bên ngoài, gồm Persistence, Repositories và Services.
- `tests`: kiểm thử riêng cho Domain, Application và Infrastructure.

Luồng xử lý thường đi từ Presentation đến Application, rồi dùng Domain và các hợp đồng repository. Infrastructure hiện thực các hợp đồng đó để kết nối dữ liệu.

## Trạng thái

Đây mới là bộ khung thư mục. Solution, project C#, mã nguồn, SQL và tài liệu nộp sẽ được thêm khi có nội dung thực tế. SQL của gói nộp nằm trong `../Database/`; không đặt mã nguồn C# trong thư mục Database.
