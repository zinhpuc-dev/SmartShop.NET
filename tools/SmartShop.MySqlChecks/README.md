# Kiểm tra MySQL phần của Triết

Bộ kiểm tra tích hợp cho phần 5, 6, 7 và 15, chạy BUS/DAO thật trên MySQL. Tách khỏi SmokeTests để các kiểm tra thường ngày không cần quyền tạo database.

## Chạy

Cần MySQL đang chạy và tài khoản có quyền tạo/xóa database kiểm thử. Từ gốc repository:

```powershell
dotnet run --project tools/SmartShop.MySqlChecks -- --local-secrets
```

`--local-secrets` dùng host/port từ `ConnectionStrings:SmartShop`, cùng mật khẩu root đã lưu riêng tại `LocalMySQL:RootPassword` trong User Secrets `SmartShop.Admin.Local`. Cờ này dành cho cấu hình cục bộ đã được thiết lập; không đưa thông tin đăng nhập vào Git.

Máy khác có thể đặt biến môi trường `SMARTSHOP_TEST_ADMIN_CONNECTION` rồi chạy không có `--local-secrets`. Tài khoản cần đủ quyền tạo schema, bảng, đọc/ghi dữ liệu và xóa database kiểm thử. Chuỗi kết nối không được in ra màn hình. Database có sẵn trong chuỗi đầu vào không được dùng làm đích ghi.

## Phạm vi

- Mỗi lần chạy tự tạo database `smartshop_test_triet_<GUID>`, bỏ tên database trong kết nối đầu vào. Không nhận tên database kiểm thử tùy ý.
- Chỉ trích 19 câu `CREATE TABLE` từ schema gốc để tạo bảng; không thực thi cả script, `USE`, `DROP TABLE`, dữ liệu mẫu hoặc migration kho. Nếu số câu DDL không khớp, dừng để rà lại bộ kiểm tra.
- Thêm/sửa/xóa cả ba danh mục, dữ liệu tiếng Việt/dấu nháy, lưu lại không đổi nội dung với `UseAffectedRows=true`, từ chối dữ liệu sai và giữ cache.
- Xóa hồ sơ có giỏ/đơn hàng, hóa đơn hoặc phiếu nhập liên kết: xác nhận lỗi FK MySQL 1451 được chuyển thành thông báo nghiệp vụ và không làm mất cache.
- Lỗi UNIQUE email khách hàng/nhân viên khi INSERT/UPDATE từ cache cũ: xác nhận lỗi 1062, giữ nguyên dữ liệu/cache; giữ email của chính bản ghi và khóa mã khi sửa.
- Hai BUS khách hàng thêm cùng mã bằng hai tác vụ song song: đúng một lần ghi thành công; phiên thua không thêm cache. Kiểm tra trùng mã nhân viên/NCC qua hai phiên có cache cũ.
- `finally` chỉ xóa database tên ngẫu nhiên do chính lần chạy hiện tại tạo thành công. Nếu tiến trình bị kết thúc cưỡng bức, có thể còn database kiểm thử; tên được in khi bắt đầu để nhận diện.

Đây là kiểm tra tích hợp nghiệp vụ danh mục và ràng buộc, không phải nghiệm thu giao diện, bảo mật đăng nhập, tải lớn, toàn bộ kiểu tranh chấp cập nhật hoặc rollback nghiệp vụ nhiều bảng. Các lỗi UPDATE được kiểm tra ở mức một câu lệnh bị MySQL từ chối, không coi là đã thử transaction nhiều bước.
