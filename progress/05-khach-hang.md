# Quản lý Khách hàng

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm

**Cập nhật lần cuối:** 17/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Vinh (UNIQUE) |
| Yêu cầu / Use Case | UC03, UC21 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/05-khach-hang/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 17/11/2026

Đối chiếu mẫu c-3-layer, giữ Blazor web/mobile và sườn GUI/BUS/DAO/DTO. KhachHangBUS/KhachHangDAO dùng MyConnection chung ở DAO để tạo/mở kết nối; using giải phóng sau mỗi thao tác. LayTheoMa duyệt ArrayList tại BUS, không đọc DAO theo từng mã. Chỉ cập nhật cache sau khi DAO ghi thành công; TaiLai làm danh sách được nạp lại ở lần đọc tiếp theo.

### Bổ sung thêm–sửa–xóa theo mẫu — 17/11/2026

- GUI tách rõ nhánh Thêm/Sửa trong hàm Luu, giữ form/component Blazor dùng chung và các hợp đồng BUS hiện có.
- Thêm: BUS kiểm tra dữ liệu và mã/email theo ràng buộc hiện có, tạo DTO riêng, gọi DAO.Them rồi mới thêm kết quả vào ArrayList.
- Sửa: BUS lấy hồ sơ từ danh sách, xác định vị trí bằng IndexOf, kiểm tra mã bị khóa và dữ liệu, tạo DTO mới, gọi DAO.Sua rồi thay phần tử tại vị trí đó. Bỏ cách cập nhật bằng with/SingleOrDefault để luồng dễ đối chiếu mẫu.
- Xóa: BUS tìm hồ sơ, gọi DAO.Xoa rồi mới Remove hồ sơ khỏi ArrayList. Nếu DAO báo lỗi, danh sách BUS được giữ nguyên.
- Giữ SQL có tham số, lớp MyConnection và xử lý lỗi hiện có. Không đổi nền tảng web/mobile hoặc sườn GUI/BUS/DAO/DTO.
- Kiểm tra lại sau chỉnh CRUD: build solution 0 lỗi/0 cảnh báo, toàn bộ 123 checks hiện có đạt. Đây là chỉnh cách viết/luồng gọi, không bổ sung checks mới trong đợt CRUD; chưa nghiệm thu ghi MySQL hoặc thao tác trình duyệt mới.

### Phạm vi kiểm chứng đợt này

- Build solution: 0 lỗi, 0 cảnh báo; 123 checks tự động đạt. Bổ sung 14 checks về tra cứu mã/cache/không gọi DAO đọc từng mã và 5 checks cấu hình kết nối chung; các checks nghiệp vụ dùng DAO giả lập.
- Kiểm tra chỉ đọc MySQL thật qua MyConnection: kết nối utf8mb4, đọc được 10 khách hàng, 10 nhân viên và 10 nhà cung cấp qua ba DAO đã chỉnh. Không ghi dữ liệu hoặc sửa schema.
- Cache theo phiên: thay đổi từ phiên khác chỉ hiện sau khi tải lại; không coi dữ liệu trong ArrayList là bản đọc CSDL mới nhất.
- Đã sửa mô tả cũ trong nháp docs/bao-cao/III-triet.md để khớp code và cách áp dụng mẫu cho web/mobile. Chưa coi đây là báo cáo cuối đã nghiệm thu.
- Còn nghiệm thu tương tác trình duyệt, CRUD/khóa ngoại/đồng thời trên MySQL và ảnh mới. Không đánh dấu toàn bộ chức năng hoàn tất.

## Mốc trước — 08/11/2026

CustomerBUS → ICustomerRepository/CustomerDAO → KHACH_HANG; đã có CRUD, validation, tìm kiếm và Reload cache. Đã đọc 10 hồ sơ. Schema không có trạng thái khóa tài khoản; không coi cờ demo cũ là đã tích hợp. Chưa thử ghi/xóa thật.

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
- Tách Customer/CustomerDraft, ICustomerBUS/CustomerBUS và ICustomerRepository/DemoCustomerRepository; đăng ký dependency injection theo phiên.
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
| 17/11/2026 | Triết | Chỉnh thêm–sửa–xóa sát mẫu: GUI tách nhánh Luu; BUS tạo DTO rõ ràng, sửa ArrayList bằng IndexOf và xóa bằng Remove sau khi DAO thành công | Codex | Giữ web/mobile, validation, khóa mã, SQL tham số và hợp đồng lớp; build 0 lỗi/0 cảnh báo, 123 checks hiện có đạt. Chưa nghiệm thu ghi MySQL hoặc tương tác UI |
| 17/11/2026 | Triết | Đối chiếu mẫu 3 lớp: KhachHangBUS/KhachHangDAO, kết nối MyConnection chung, tra cứu mã tại ArrayList BUS; đồng bộ nháp báo cáo | Codex | Build 0 lỗi/0 cảnh báo, 123 checks đạt; MySQL chỉ đọc 10 KH/10 NV/10 NCC. Chưa nghiệm thu ghi hoặc tương tác UI; giữ web/mobile |
| 03/11/2026 | Triết | Hoàn thành bản demo web của phần 05: CRUD, hồ sơ, tìm/lọc, trạng thái demo, popup chọn và kiểm tra dữ liệu; tách bố cục Admin khỏi User, lưu ảnh và nháp báo cáo | Codex | Làm local trên develop, chưa commit/push. Còn SQL/MyConnection, xác thực và báo cáo chính |

| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc lịch sử (đối chiếu cập nhật phía trên)

- 03/11/2026: chờ phối hợp Vinh chốt schema, ràng buộc UNIQUE và quan hệ khách hàng–hóa đơn; hiện dùng DAO trong bộ nhớ, chưa có lưu dữ liệu qua phiên.
- Chờ tích hợp MyConnection/DAO CSDL và xác thực/phân quyền của Phúc. Cờ trạng thái hiện chưa khóa được tài khoản đăng nhập thật.
- Chưa có liên kết khóa ngoại với giao dịch để kiểm tra điều kiện xóa khách hàng đã phát sinh hóa đơn.
- Bản web dùng callback theo phiên cho popup; chưa triển khai cơ chế biến static đặc thù của mẫu desktop.
