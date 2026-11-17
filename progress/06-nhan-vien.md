# Quản lý Nhân viên

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 17/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Vinh (UNIQUE) |
| Yêu cầu / Use Case | UC03 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/06-nhan-vien/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 17/11/2026

Tái sử dụng nghiệp vụ qua DanhMucBUS<TDto,TForm>. Các hàm Them/Sua/Xoa có đầy đủ luồng xử lý và được khai báo ngay trong file BUS/admin/NhanVienBUS.cs theo yêu cầu người dùng; các hàm đó gọi helper chung để kiểm tra dữ liệu/trùng mã, dùng DAO và cập nhật ArrayList sau khi ghi thành công.

- Lớp cơ sở dùng chung cache ArrayList, TaiLai, tìm mã/tìm đa trường, DataAnnotations, kiểm tra email, trùng mã/email và gợi ý mã. Cache riêng từng instance, không static.
- NhanVienBUS giữ chuẩn hóa, ánh xạ DTO, trường tìm kiếm và bộ lọc riêng. Khách hàng/nhân viên tiếp tục kiểm tra email duy nhất.
- Gợi ý mã lập HashSet một lần thay vì quét toàn danh sách cho từng mã thử, vẫn lấy khoảng trống đầu tiên và giữ định dạng NV + 6 chữ số.
- IDanhMucDAO<TDto> gom hợp đồng CRUD; interface DAO riêng kế thừa hợp đồng chung. DTO thực hiện IHoSoLienHe để cung cấp thuộc tính chung, không chứa SQL. GUI và chữ ký các hàm đang gọi không đổi.
- Build solution 0 lỗi/0 cảnh báo; 129 smoke checks và 57 checks MySQL tích hợp đạt. CSDL kiểm thử đã được dọn; không sửa database ứng dụng.
- Đã cập nhật báo cáo III và tài liệu bảo vệ cho đúng vị trí CRUD/lớp cơ sở. Còn nghiệm thu UI/ảnh và review báo cáo cuối; trạng thái vẫn Đang làm.

## Mốc trước — 17/11/2026

Bổ sung bộ kiểm tra tích hợp tools/SmartShop.MySqlChecks dùng MySQL thật trên database ngẫu nhiên riêng, dựng 19 bảng từ DDL gốc rồi tự dọn. Không ghi vào database ứng dụng hoặc chạy migration kho.

- 54 checks MySQL toàn bộ đạt. Riêng NV đã kiểm tra thêm/sửa/xóa, lưu không đổi dữ liệu với UseAffectedRows=true, dữ liệu sai, khóa mã, UNIQUE email khi INSERT/UPDATE với cache cũ và giữ email chính mình.
- FK hóa đơn chặn xóa NV bằng lỗi 1451 thật; lỗi ghi không đổi CSDL/cache. Hai phiên có cache cũ thêm trùng mã được PRIMARY KEY chặn và trả thông báo qua DAO.
- 123 smoke checks đạt; build bộ kiểm tra và solution sau dọn: 0 lỗi/0 cảnh báo. Đã bổ sung bằng chứng vào bản nháp báo cáo III.
- Còn liên kết tài khoản nếu yêu cầu, nghiệm thu thao tác web/mobile, ảnh mới và chốt báo cáo cuối. Không coi chức vụ hồ sơ là phân quyền đăng nhập nhiều vai trò.

## Mốc trước — 17/11/2026

Đối chiếu mẫu c-3-layer, giữ Blazor web/mobile và sườn GUI/BUS/DAO/DTO. NhanVienBUS/NhanVienDAO dùng MyConnection chung ở DAO để tạo/mở kết nối; using giải phóng sau mỗi thao tác. LayTheoMa duyệt ArrayList tại BUS, không đọc DAO theo từng mã. Chỉ cập nhật cache sau khi DAO ghi thành công; TaiLai làm danh sách được nạp lại ở lần đọc tiếp theo.

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

EmployeeBUS → IEmployeeRepository/EmployeeDAO → NHAN_VIEN; mapping Code/Name/Position(ChucVu)/Phone/Email. Không giả định địa chỉ/tình trạng làm việc vì schema không có. Đã đọc 10 hồ sơ, chưa thử ghi/xóa hoặc liên kết tài khoản nhân viên.

### Mốc triển khai và kiểm chứng

- [x] Đọc được 10 nhân viên MySQL, có ChucVu.
- [x] Có mã CRUD, validation, khóa mã và Reload cache.
- [x] Kiểm thử ghi và ràng buộc hóa đơn qua BUS/DAO trên MySQL riêng.
- [ ] Liên kết tài khoản nhân viên nếu yêu cầu và nghiệm thu thao tác giao diện.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [x] CRUD Nhân viên theo GUI-BUS-DAO (đã có DAO MySQL; thao tác ghi chưa nghiệm thu)
- [x] Kiểm tra trùng mã, field bắt buộc, field khóa không cho sửa
- [x] Form tìm nhân viên cho nút "..." (popup web trong Admin)
- [x] Chụp ảnh form cho báo cáo
- [x] Đã có ảnh form trong `docs/images/06-nhan-vien/`
- [x] Đã viết phần báo cáo tương ứng (bản nháp III-triet.md đã có nội dung; còn review, ảnh mới và chốt bản cuối)

### Phạm vi trước 08/11/2026 (lịch sử)

- Trang `/admin/employees` thuộc Admin, có menu riêng trên desktop và điện thoại; hỗ trợ thêm, sửa, xóa có xác nhận, xem hồ sơ và đổi tình trạng làm việc.
- Tách DTO Employee/EmployeeDraft, BUS IEmployeeBUS/EmployeeBUS, hợp đồng IEmployeeRepository và DAO DemoEmployeeRepository; ghép qua dependency injection theo phiên, BUS không tham chiếu DAO cụ thể.
- BUS giữ ArrayList, tìm trên danh sách đã nạp một lần theo mã, tên, điện thoại, email, địa chỉ; hỗ trợ tiếng Việt không dấu và lọc tình trạng làm việc.
- Chặn mã trùng không phân biệt hoa/thường; kiểm tra dữ liệu ở form và BUS; giữ nguyên mã, ngày tạo khi sửa. Hủy form và lỗi ghi DAO không làm thay đổi dữ liệu đã lưu trong BUS.
- Dùng lại DataTable, ValidatedForm, FormField, ModalDialog và kiểu trình bày hiện có. SearchText được dùng chung cho khách hàng và nhân viên.
- EmployeePicker/EmployeeSelector trả nhân viên đã chọn bằng callback, có tùy chọn chỉ chọn người đang làm việc để dùng lại trong nghiệp vụ quản trị.
- Đã lưu 5 ảnh danh sách, form sửa, popup và giao diện mobile tại `docs/images/06-nhan-vien/`.
- Nháp báo cáo: `docs/bao-cao/III-triet-nhan-vien.md`; chưa tick báo cáo chính vì chưa gộp vào `docs/bao-cao/III-triet.md` và chưa hoàn tất tích hợp CSDL.
- Build thành công, 0 lỗi/0 cảnh báo; 29 kiểm tra Nhân viên và 48 kiểm tra trước đó đều đạt. Đã kiểm tra thao tác trên trình duyệt và bố cục 390 × 844 không tràn ngang.

## Tiêu chí nghiệm thu

Thêm, sửa, xóa, tìm được; không trùng mã

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 17/11/2026 | Triết | Tái sử dụng helper qua DanhMucBUS; giữ Them/Sua/Xoa viết đầy đủ trong NhanVienBUS; dùng chung hợp đồng DAO/DTO và tối ưu gợi ý mã bằng HashSet | Codex | Build 0 lỗi/0 cảnh báo, 129 smoke/57 MySQL checks đạt; giữ web/mobile và quy tắc riêng. Còn UI/ảnh và nghiệm thu cuối |
| 17/11/2026 | Triết | Kiểm thử CRUD NV, UNIQUE email/mã, khóa mã và FK hóa đơn trên MySQL riêng; bổ sung báo cáo | Codex | 54 checks MySQL toàn bộ và 123 smoke checks đạt; DB thử đã dọn. Còn UI/ảnh và liên kết tài khoản nếu yêu cầu |
| 17/11/2026 | Triết | Chỉnh thêm–sửa–xóa sát mẫu: GUI tách nhánh Luu; BUS tạo DTO rõ ràng, sửa ArrayList bằng IndexOf và xóa bằng Remove sau khi DAO thành công | Codex | Giữ web/mobile, validation, khóa mã, SQL tham số và hợp đồng lớp; build 0 lỗi/0 cảnh báo, 123 checks hiện có đạt. Chưa nghiệm thu ghi MySQL hoặc tương tác UI |
| 17/11/2026 | Triết | Đối chiếu mẫu 3 lớp: NhanVienBUS/NhanVienDAO, kết nối MyConnection chung, tra cứu mã tại ArrayList BUS; đồng bộ nháp báo cáo | Codex | Build 0 lỗi/0 cảnh báo, 123 checks đạt; MySQL chỉ đọc 10 KH/10 NV/10 NCC. Chưa nghiệm thu ghi hoặc tương tác UI; giữ web/mobile |
| 03/11/2026 | Triết | Hoàn thành bản demo web phần 06: CRUD, hồ sơ, tình trạng làm việc, tìm/lọc, popup chọn, kiểm tra dữ liệu; tái sử dụng component, lưu ảnh và nháp báo cáo | Codex | Local trên develop, chưa commit/push. Còn CSDL thật, liên kết tài khoản/giao dịch và báo cáo chính |

| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc lịch sử (đối chiếu cập nhật phía trên)

- 03/11/2026: dữ liệu hiện nằm trong bộ nhớ theo phiên, chưa lưu bền vững; chờ phối hợp Vinh chốt schema, UNIQUE và tích hợp SQL/MyConnection.
- Tình trạng làm việc chưa liên kết đăng nhập/phân quyền; cần phối hợp Phúc khi tích hợp tài khoản.
- Chưa có quan hệ hóa đơn/phiếu nhập để chặn xóa nhân viên đã phát sinh giao dịch; cần bổ sung khi tích hợp CSDL.
- Popup web dùng callback theo phiên; chưa triển khai cơ chế biến static đặc thù của mẫu desktop.
- Chưa gộp nháp vào báo cáo III chính.
