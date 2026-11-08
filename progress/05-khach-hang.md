# Quản lý Khách hàng

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm

**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Vinh (UNIQUE) |
| Yêu cầu / Use Case | UC03, UC21 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` (local, chưa commit/push) |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/05-khach-hang/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

CustomerService → ICustomerRepository/CustomerDAO → KHACH_HANG; đã có CRUD, validation, tìm kiếm và Reload cache. Đã đọc 10 hồ sơ. Schema không có trạng thái khóa tài khoản; không coi cờ demo cũ là đã tích hợp. Chưa thử ghi/xóa thật.

### Mốc triển khai và kiểm chứng

- [x] Đọc được 10 khách hàng MySQL qua trang được bảo vệ.
- [x] Có mã CRUD, khóa mã khi sửa, tìm không dấu và tải lại cache.
- [ ] Kiểm thử ghi CRUD và chặn xóa có liên kết trên database riêng.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [x] CRUD Khách hàng theo GUI-BUS-DAO (đã có DAO MySQL; thao tác ghi chưa nghiệm thu)
- [ ] Hồ sơ khách hàng, đổi trạng thái tài khoản
- [x] Kiểm tra trùng mã, field bắt buộc, field khóa không cho sửa
- [x] Form tìm khách hàng cho nút "..." (popup web trong Admin)
- [x] Chụp ảnh form cho báo cáo
- [x] Đã có ảnh form trong `docs/images/05-khach-hang/`
- [ ] Đã viết phần báo cáo tương ứng (III (Triết))

### Phạm vi trước 08/11/2026 (lịch sử)

- Trang `/admin/customers` thuộc project Admin, có bố cục và menu quản trị riêng; giao diện User không có quản lý hoặc tra cứu danh sách khách hàng.
- Đã có xem hồ sơ và khóa/mở cờ trạng thái trong dữ liệu demo. Mục trạng thái tài khoản ở trên chưa tick vì chưa tích hợp hệ thống đăng nhập thật.
- Tách Customer/CustomerDraft, ICustomerService/CustomerService và ICustomerRepository/DemoCustomerRepository; đăng ký dependency injection theo phiên.
- BUS giữ ArrayList và tìm kiếm trên danh sách đã nạp, không đọc lại DAO khi gõ; hỗ trợ tên tiếng Việt không dấu, mã, điện thoại, email, địa chỉ và lọc trạng thái.
- Chặn mã trùng không phân biệt hoa/thường, khóa mã và ngày tạo khi sửa; kiểm tra dữ liệu cả ở form và BUS. Hủy form hoặc lưu lỗi không thay đổi danh sách BUS.
- CustomerPicker/CustomerSelector trả dữ liệu bằng callback và điền ô khách hàng đã chọn trong Admin; component có thể dùng lại cho các nghiệp vụ quản trị.
- Đã có nháp báo cáo tại `docs/bao-cao/III-triet-khach-hang.md`; chưa tick báo cáo chính vì chưa gộp vào `docs/bao-cao/III-triet.md` và chưa chốt phần CSDL thật.
- Build thành công, 0 lỗi/0 cảnh báo. 29 kiểm tra Khách hàng và 19 kiểm tra nghiệp vụ/DI trước đó đã qua; đã kiểm tra form, CRUD, popup và bố cục desktop/mobile trên trình duyệt.

## Tiêu chí nghiệm thu

Thêm, sửa, xóa, tìm được; không trùng mã

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 03/11/2026 | Triết | Hoàn thành bản demo web của phần 05: CRUD, hồ sơ, tìm/lọc, trạng thái demo, popup chọn và kiểm tra dữ liệu; tách bố cục Admin khỏi User, lưu ảnh và nháp báo cáo | Codex | Làm local trên develop, chưa commit/push. Còn SQL/MyConnection, xác thực và báo cáo chính |

| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc lịch sử (đối chiếu cập nhật phía trên)

- 03/11/2026: chờ phối hợp Vinh chốt schema, ràng buộc UNIQUE và quan hệ khách hàng–hóa đơn; hiện dùng DAO trong bộ nhớ, chưa có lưu dữ liệu qua phiên.
- Chờ tích hợp MyConnection/DAO CSDL và xác thực/phân quyền của Phúc. Cờ trạng thái hiện chưa khóa được tài khoản đăng nhập thật.
- Chưa có liên kết khóa ngoại với giao dịch để kiểm tra điều kiện xóa khách hàng đã phát sinh hóa đơn.
- Bản web dùng callback theo phiên cho popup; chưa triển khai cơ chế biến static đặc thù của mẫu desktop.
