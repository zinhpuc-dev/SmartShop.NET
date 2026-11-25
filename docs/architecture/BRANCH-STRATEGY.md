# Quy ước nhánh

## `main`

`main` chỉ chứa nền tảng và nội dung dùng chung: giao diện nền, thư viện chia sẻ khi thật sự cần, yêu cầu sản phẩm, dữ liệu, tài liệu cài đặt và cấu hình chung. Không đặt backend đặc thù của OOAD hoặc kiến trúc ba lớp vào nhánh này.

## `OOAD`

Tạo từ `main`. Giữ phần giao diện và dữ liệu chung; bổ sung backend được thiết kế theo OOAD cùng các diagram của môn học. Không đưa cấu trúc backend ba lớp của nhánh `CSharp` vào đây.

Gợi ý tổ chức backend: `src/SmartShop.Domain`, `src/SmartShop.Application`, `src/SmartShop.Infrastructure`. Lưu diagram và giải thích thiết kế trong `docs/architecture/OOAD/`.

## `CSharp`

Tạo từ `main`. Giữ phần giao diện và dữ liệu chung; bổ sung backend C# theo ba lớp. Không đưa backend và diagram đặc thù của nhánh `OOAD` vào đây.

Gợi ý tổ chức backend: `src/SmartShop.BUS`, `src/SmartShop.DAO`, `src/SmartShop.DTO`. Tài liệu nộp riêng của môn C# đặt trong `docs/CSharp/`.

## Đồng bộ phần chung

Khi có thay đổi thực sự dùng chung, đưa thay đổi vào `main`, sau đó merge `main` vào từng nhánh môn học. Không merge ngược toàn bộ một nhánh môn học vào nhánh còn lại. Mỗi nhánh môn học phải có đủ mã nguồn và hướng dẫn cần thiết để chạy, trình bày và nộp độc lập.
