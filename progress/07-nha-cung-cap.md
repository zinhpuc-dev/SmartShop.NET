# Quản lý Nhà cung cấp

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 17/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Vinh (UNIQUE) |
| Yêu cầu / Use Case | Danh mục NCC |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/07-nha-cung-cap/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 17/11/2026

Bổ sung tools/SmartShop.MySqlChecks, kiểm thử MySQL thật trên database riêng được dựng từ 19 câu CREATE TABLE gốc và dọn trong finally. Không ghi vào database ứng dụng, không sửa schema đang dùng hoặc chạy migration kho.

- 54 checks MySQL toàn bộ đạt. Riêng NCC đã kiểm tra CRUD, tiếng Việt/dấu nháy, lưu không đổi dữ liệu, chặn dữ liệu sai và khóa mã khi sửa.
- FK phiếu nhập chặn xóa NCC bằng lỗi 1451 thật; PRIMARY KEY chặn trùng mã từ phiên có cache cũ. Sửa hồ sơ bị phiên khác xóa trả lỗi không tìm thấy, không tạo lại bản ghi/cache giả; tải lại loại hồ sơ cũ khỏi cache.
- Schema hiện tại không có UNIQUE email NCC; không bổ sung yêu cầu này ngoài phân công.
- 123 smoke checks đạt; build bộ kiểm tra và solution sau dọn: 0 lỗi/0 cảnh báo. Báo cáo đã bổ sung bằng chứng này, còn review bản cuối.
- Còn nghiệm thu popup/thao tác giao diện, ảnh riêng phần 7 và các kiểu tranh chấp khác.

## Mốc trước — 17/11/2026

Đối chiếu mẫu c-3-layer, giữ Blazor web/mobile và sườn GUI/BUS/DAO/DTO. NhaCungCapBUS/NhaCungCapDAO dùng MyConnection chung ở DAO để tạo/mở kết nối; using giải phóng sau mỗi thao tác. LayTheoMa duyệt ArrayList tại BUS, không đọc DAO theo từng mã. Chỉ cập nhật cache sau khi DAO ghi thành công; TaiLai làm danh sách được nạp lại ở lần đọc tiếp theo.

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

## Mốc trước — 12/11/2026

NhaCungCapBUS → INhaCungCapDAO/NhaCungCapDAO → NHA_CUNG_CAP. Đã gắn OChonNhaCungCap và TimChonNhaCungCap vào form tạo phiếu nhập tại `/admin/inventory`, thay dropdown nhà cung cấp.

### Mốc triển khai và kiểm chứng

- [x] Đã đọc được 10 nhà cung cấp MySQL ở đợt kiểm tra trước.
- [x] Có CRUD/tìm kiếm và tích hợp popup chọn nhà cung cấp vào phiếu nhập ở mức code.
- [x] Callback điền SupplierCode và thông tin chỉ đọc; lưu phiếu thành công sẽ xóa lựa chọn cho phiếu mới. Có khóa nút khi tải dữ liệu lỗi và thông báo khi không đọc được hồ sơ đã chọn.
- [x] Build solution: 0 lỗi, 0 cảnh báo; 78 smoke checks hiện có đạt. Các checks này không nghiệm thu tương tác popup hay ghi phiếu MySQL.
- [ ] Kiểm thử thao tác popup trên trình duyệt, ghi/xóa có liên kết và chụp ảnh mới. Browser Use từ chối quyền mở địa chỉ cục bộ trong đợt này.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [x] CRUD Nhà cung cấp theo GUI-BUS-DAO (đã có DAO MySQL; thao tác ghi chưa nghiệm thu)
- [x] Kiểm tra trùng mã, field bắt buộc
- [x] Form tìm nhà cung cấp cho nút "..." ở phiếu nhập (đã gắn component và callback; còn nghiệm thu thao tác trực tiếp)
- [ ] Chụp ảnh form cho báo cáo
- [ ] Đã có ảnh form trong `docs/images/07-nha-cung-cap/`
- [x] Đã viết phần báo cáo tương ứng (bản nháp III-triet.md đã có nội dung; còn review, ảnh mới và chốt bản cuối)

### Phạm vi trước 08/11/2026 (lịch sử)

- Trang `/admin/suppliers` có trong menu Admin. Hỗ trợ thêm, sửa, xóa có xác nhận và tìm theo mã, tên, điện thoại, email hoặc địa chỉ.
- GUI dùng lại DataTable, ValidatedForm, FormField và ModalDialog. SupplierSelector và SupplierPicker trả nhà cung cấp đã chọn qua callback để có thể gắn vào phiếu nhập.
- BUS giữ ArrayList, tìm không dấu bằng SearchText, kiểm tra dữ liệu bắt buộc và mã trùng không phân biệt hoa/thường. Mã và ngày tạo được giữ nguyên khi sửa.
- DAO demo lưu trong bộ nhớ theo phiên qua ISupplierRepository. BUS chỉ cập nhật danh sách sau khi DAO lưu thành công.
- Build thành công với 0 lỗi, 0 cảnh báo. Toàn bộ 84 kiểm tra đạt, gồm 7 kiểm tra mới cho nhà cung cấp. Đã xác nhận trang Nhà cung cấp và các trang Admin liên quan trả HTTP 200.

## Tiêu chí nghiệm thu

Thêm, sửa, xóa, tìm được; không trùng mã

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 17/11/2026 | Triết | Kiểm thử CRUD NCC, FK phiếu nhập, mã trùng và sửa hồ sơ đã bị phiên khác xóa; bổ sung báo cáo | Codex | 54 checks MySQL toàn bộ và 123 smoke checks đạt; DB thử đã dọn. Còn nghiệm thu popup/UI, ảnh mới và bản báo cáo cuối |
| 17/11/2026 | Triết | Chỉnh thêm–sửa–xóa sát mẫu: GUI tách nhánh Luu; BUS tạo DTO rõ ràng, sửa ArrayList bằng IndexOf và xóa bằng Remove sau khi DAO thành công | Codex | Giữ web/mobile, validation, khóa mã, SQL tham số và hợp đồng lớp; build 0 lỗi/0 cảnh báo, 123 checks hiện có đạt. Chưa nghiệm thu ghi MySQL hoặc tương tác UI |
| 17/11/2026 | Triết | Đối chiếu mẫu 3 lớp: NhaCungCapBUS/NhaCungCapDAO, kết nối MyConnection chung, tra cứu mã tại ArrayList BUS; đồng bộ nháp báo cáo | Codex | Build 0 lỗi/0 cảnh báo, 123 checks đạt; MySQL chỉ đọc 10 KH/10 NV/10 NCC. Chưa nghiệm thu ghi hoặc tương tác UI; giữ web/mobile |
| 12/11/2026 | Triết | Gắn popup tìm/chọn nhà cung cấp vào phiếu nhập; khóa nút khi tải lỗi, hiển thị thông tin chỉ đọc và bổ sung xử lý lỗi chọn | Codex | Build 0 lỗi/0 cảnh báo, 78 checks hiện có đạt. Chưa nghiệm thu UI/ghi MySQL; chưa có ảnh mới |
| 04/11/2026 | Triết | Triển khai bản demo phần 07 với CRUD, tìm kiếm trên ArrayList, kiểm tra dữ liệu và popup chọn nhà cung cấp | Codex | Đang làm, 2/6 mục. Popup đã có trong Admin, chờ gắn vào phiếu nhập |

| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc lịch sử (đối chiếu cập nhật phía trên)

- Phối hợp Vinh để tích hợp SQL/MyConnection, ràng buộc UNIQUE và quan hệ với phiếu nhập. Hiện chưa lưu dữ liệu qua phiên và chưa kiểm tra được điều kiện xóa nhà cung cấp đã có phiếu nhập.
- Gắn popup chọn nhà cung cấp khi chức năng phiếu nhập sẵn sàng.
- Còn chụp ảnh form và viết phần báo cáo III.
