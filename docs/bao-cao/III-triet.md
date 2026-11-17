# III. Các chức năng chính — Triết

Cập nhật 17/11/2026 theo mã nguồn hiện tại, đối chiếu mẫu c-3-layer. Giữ nền tảng Blazor web/mobile và phân công thành viên. Đã bổ sung kiểm tra ghi MySQL trên database riêng cho ba danh mục và ràng buộc; chưa nghiệm thu toàn bộ thao tác giao diện hoặc mọi kiểu tranh chấp. Build và kiểm tra với DAO giả lập không thay thế kiểm thử ghi trên CSDL thật.

## Cấu trúc ba lớp và DTO

- GUI: các trang và component Razor nhận đầu vào, hiển thị dữ liệu/thông báo, gọi BUS qua interface; không viết SQL hoặc mở kết nối MySQL trong màn hình danh mục.
- BUS: KhachHangBUS, NhanVienBUS, NhaCungCapBUS kế thừa DanhMucBUS<TDto,TForm>, dùng chung xử lý nghiệp vụ và giữ ArrayList riêng theo từng instance; kiểm tra dữ liệu hợp lệ, mã duy nhất, khóa mã khi sửa và xử lý tìm kiếm. Khách hàng/nhân viên còn kiểm tra trùng email không rỗng.
- DAO: KhachHangDAO, NhanVienDAO, NhaCungCapDAO thực hiện đọc/thêm/sửa/xóa bằng SQL có tham số. Ba DAO dùng lớp MyConnection chung để tạo/mở kết nối; dùng using để giải phóng sau mỗi thao tác. Chuỗi kết nối nhận từ cấu hình ngoài mã nguồn, mặc định utf8mb4 nếu chưa chỉ định bộ mã.
- DTO: KhachHangDTO, NhanVienDTO, NhaCungCapDTO truyền dữ liệu; các lớp DuLieuKhachHang/DuLieuNhanVien/DuLieuNhaCungCap chứa dữ liệu form và ràng buộc DataAnnotations. DTO không truy cập MySQL.
- GUI → BUS → hợp đồng DAO → DAO MySQL là luồng nghiệp vụ. KhoiDong.cs chỉ ghép các implementation qua DI. BUS không tham chiếu assembly DAO cụ thể.
- Tương ứng mẫu desktop: Razor/component thay form và ListView; dữ liệu BUS đóng gói theo phiên thay danh sách static dùng chung mọi người. ArrayList, tra cứu mã trong danh sách BUS và lớp MyConnection giữ cách phân chia trách nhiệm của mẫu.

## Tái sử dụng và tối ưu BUS

DanhMucBUS<TDto,TForm> là lớp cơ sở dùng chung cache ArrayList, tra mã, tìm đa trường, gợi ý mã, kiểm tra DataAnnotations, định dạng email và trùng mã/email. Các hàm Them/Sua/Xoa được viết rõ trong từng file BUS con, gọi lại helper chung qua protected. BUS con còn chuẩn hóa dữ liệu, tạo DTO, chỉ định trường riêng và bộ lọc nâng cao. Quy tắc NCC cho phép email trùng được override riêng; không áp dụng nhầm quy tắc email KH/NV cho NCC.

IDanhMucDAO<TDto> gom hợp đồng CRUD, các interface DAO danh mục riêng kế thừa hợp đồng này. IHoSoLienHe cung cấp thuộc tính chung cho BUS; không đưa SQL vào DTO/BUS. Giao diện, đăng ký DI và chữ ký hàm gọi từ GUI được giữ nguyên.

GoiYMa tạo HashSet mã hiện có một lần để tránh lặp quét ArrayList cho từng mã thử, vẫn trả khoảng trống nhỏ nhất và giữ tiền tố/độ dài mã riêng của từng danh mục. Cache không static và chỉ cập nhật sau khi DAO ghi thành công.

Sau khi tách BUS chung: build solution 0 lỗi/0 cảnh báo; 129 smoke checks và 57 checks MySQL tích hợp đạt. Bổ sung kiểm tra khoảng trống mã, tiền tố/độ dài mã, cache riêng từng instance và quy tắc NCC cho phép email trùng nhưng vẫn từ chối email sai định dạng. Database thử đã được dọn. Đợt này không thay đổi GUI hoặc nghiệm thu lại thao tác trình duyệt.

## Quản lý khách hàng

1. **Giới thiệu:** Danh sách, tìm kiếm và CRUD khách hàng qua GUI → BUS → DAO MySQL.
2. **Giao diện:** Trang /admin/customers. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin hồ sơ phù hợp KHACH_HANG; kiểm tra bắt buộc và định dạng tại BUS.
5. **Đầu ra:** Danh sách và thông báo thành công/lỗi phù hợp thao tác.
6. **Xử lý:** BUS nạp KHACH_HANG qua DAO vào ArrayList khi cần; tìm tên và mã trên danh sách đã nạp. Khi thêm/sửa/xóa, BUS kiểm tra đầu vào rồi gọi DAO; chỉ cập nhật ArrayList sau khi DAO thành công. TaiLai xóa cache để lần đọc tiếp theo tải lại. Đợt kiểm chứng trước đã đọc HTTP thành công 10 hồ sơ; chưa nghiệm thu CRUD MySQL đầy đủ. Khóa/mở tài khoản trong demo cũ không phải tính năng đã tích hợp vào schema hiện tại.

## Quản lý nhân viên

1. **Giới thiệu:** Danh sách, tìm kiếm và CRUD nhân viên.
2. **Giao diện:** Trang /admin/employees. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin NHAN_VIEN, bao gồm ChucVu; BUS kiểm tra dữ liệu.
5. **Đầu ra:** Danh sách và phản hồi thao tác.
6. **Xử lý:** GUI chỉ gọi BUS; BUS gọi DAO để nạp ArrayList hoặc ghi MySQL. Tra cứu mã, tìm kiếm theo tên/chức vụ/điện thoại/email thực hiện trong BUS. Danh sách cập nhật sau khi ghi thành công và chỉ tải lại khi TaiLai hoặc khởi tạo lại danh sách. Đợt kiểm chứng trước đã đọc 10 hồ sơ qua HTTP; chưa nghiệm thu thao tác ghi đầy đủ. Chức vụ hồ sơ nhân viên không đồng nghĩa với phân quyền đăng nhập nhiều tài khoản.

## Quản lý nhà cung cấp

1. **Giới thiệu:** Quản lý hồ sơ nhà cung cấp.
2. **Giao diện:** Trang /admin/suppliers. Ảnh minh họa cần chụp lại từ bản hiện tại; chưa có ảnh nghiệm thu mới.
3. **Công thức:** Không có phép tính tiền.
4. **Đầu vào:** Thông tin phù hợp NHA_CUNG_CAP.
5. **Đầu ra:** Danh sách hồ sơ và thông báo thao tác.
6. **Xử lý:** NhaCungCapBUS nạp ArrayList qua NhaCungCapDAO; tra cứu mã và tìm đa trường trong BUS. Ghi thành công mới đổi cache; DAO chuyển lỗi khóa ngoại thành thông báo không thể xóa hồ sơ có phiếu nhập. Popup chọn NCC đã nối callback với SupplierCode của phiếu nhập. Đã kiểm tra CRUD, xóa có liên kết, mã trùng từ phiên khác và sửa hồ sơ bị phiên khác xóa trên MySQL kiểm thử riêng; còn nghiệm thu thao tác popup trên trình duyệt.

## Form chính và điều hướng — phần 4

1. **Giới thiệu:** Bố cục quản trị web/mobile với menu và vùng hiển thị trang con.
2. **Giao diện:** BoCucQuanTri/KhungQuanTri, menu trái trên desktop và thanh điều hướng mobile; ảnh cũ cần chụp lại theo bản hiện tại.
3. **Công thức:** Không tính toán tiền; mục đang chọn xác định theo đường dẫn hiện tại.
4. **Đầu vào:** URL, trạng thái đăng nhập và ChildContent.
5. **Đầu ra:** Tên tài khoản, menu tương ứng và trang chức năng; khi không đủ quyền hiện lối đăng nhập lại.
6. **Xử lý:** AuthorizeView dùng policy Admin hiện có để kiểm soát menu/nội dung; bảo vệ route phía server vẫn được giữ. Có kiểm tra render ẩn danh, thiếu quyền, Admin, đổi trạng thái phiên và encode tên tài khoản; chưa coi kiểm tra render là nghiệm thu cookie/phiên hết hạn thực tế.

## Tìm kiếm trên ArrayList — phần 9

1. **Giới thiệu:** Tìm danh mục tại BUS, không truy vấn lại CSDL mỗi lần gõ.
2. **Giao diện:** Ô tìm cơ bản trên danh mục và trong popup chọn hồ sơ.
3. **Công thức:** Chuẩn hóa chuỗi tiếng Việt; lấy bản ghi có ít nhất một trường chứa từ khóa. Tìm mã dùng so sánh bằng không phân biệt hoa/thường.
4. **Đầu vào:** Từ khóa, mã hoặc loại sản phẩm; ArrayList đã nạp từ DAO.
5. **Đầu ra:** Danh sách kết quả hoặc thông báo không tìm thấy mã.
6. **Xử lý:** Nạp danh sách khi cache null, duyệt trong BUS, trả kết quả cho GUI; cập nhật ArrayList sau ghi thành công. TaiLai chỉ hủy cache, lần đọc kế tiếp nạp lại. Đã có tài liệu ôn bảo vệ 10 phút tại docs/HUONG_DAN_BAO_VE_TRIET.md; không thay việc người trình bày cần tự hiểu và luyện giải thích.

## Tìm kiếm nâng cao — phần 10

1. **Giới thiệu:** Lọc nhiều điều kiện trên bốn danh mục Admin.
2. **Giao diện:** TimKiemNangCao dùng lại, thêm/bớt điều kiện và nhóm; bộ lọc chỉ áp dụng khi bấm nút.
3. **Công thức:** Kết quả = OR của các nhóm; mỗi nhóm = AND các điều kiện. NOT đảo kết quả một điều kiện. Số hỗ trợ =, <>, >, >=, <, <=; văn bản hỗ trợ Chứa, =, <>.
4. **Đầu vào:** BoLocDanhMuc, trường tìm kiếm và giá trị do người dùng nhập.
5. **Đầu ra:** Danh sách thỏa điều kiện; điều kiện sai trả thông báo và giữ bộ lọc đã áp dụng.
6. **Xử lý:** BUS kiểm tra trường, kiểu giá trị, toán tử và giới hạn nhóm trước khi lọc dữ liệu bộ nhớ. Không có phân tích biểu thức tự do hoặc ngoặc lồng tùy ý. Phần review BUS của Phúc và nghiệm thu cuối vẫn cần phối hợp.

## Ràng buộc dữ liệu — phần 15

1. **Giới thiệu:** Kiểm tra đầu vào, mã/email trùng, khóa mã khi sửa và ngăn xóa hồ sơ có liên kết.
2. **Giao diện:** DataAnnotations trên form, ô mã readonly khi sửa và thông báo nghiệp vụ từ BUS/DAO.
3. **Công thức:** Mã không trùng; email không rỗng của KH/NV không trùng sau chuẩn hóa tại BUS và theo UNIQUE MySQL. NCC không có UNIQUE email trong schema hiện tại.
4. **Đầu vào:** Dữ liệu form, mã hồ sơ đang sửa/xóa và trạng thái dữ liệu MySQL.
5. **Đầu ra:** Hồ sơ được lưu hoặc thông báo từ chối; lỗi ghi không thêm/sửa/xóa cache giả.
6. **Xử lý:** BUS kiểm tra trước khi gọi DAO; MySQL PRIMARY KEY/UNIQUE/FK xử lý các xung đột chưa có trong cache. Đã kiểm tra lỗi 1062/1451 thật và một số tình huống hai phiên bằng bộ kiểm tra MySQL riêng. Mã gợi ý chưa phải cơ chế cấp mã tự động toàn hệ thống; chưa triển khai khóa phiên bản chống ghi đè mọi kiểu sửa đồng thời hoặc nghiệm thu toàn bộ trường tính toán của module khác.

## Tìm kiếm và bố cục

- Ba danh mục dùng ArrayList riêng tại BUS. TimKiem và LayTheoMa duyệt dữ liệu đã nạp, không truy vấn lại DAO khi gõ hoặc tra mã; mã được bỏ khoảng trắng đầu/cuối và so sánh không phân biệt hoa/thường. LayTheoMa báo không tìm thấy nếu mã không có trong danh sách. Dữ liệu thay đổi từ phiên khác chỉ được phản ánh sau khi tải lại, không coi cache là bản đọc mới nhất của CSDL.
- Tìm kiếm nâng cao dùng TimKiemDanhMucNangCao chung, AND trong nhóm/OR giữa nhóm, NOT và phép so sánh phù hợp từng trường. Điều kiện được kiểm tra tại BUS trước khi lọc.
- BangDuLieu, BieuMauKiemTra, TruongNhapLieu, HopThoai và các component chọn hồ sơ được dùng lại. Popup trả DTO qua EventCallback theo phiên; popup nhân viên đã gắn vào lập hóa đơn, popup NCC vào phiếu nhập.
- KhungQuanTri hiển thị tên tài khoản từ phiên, menu desktop/mobile và vùng nội dung theo policy Admin hiện có. Đây là bố cục web/mobile, không phải MDI WinForms. Chưa có phân quyền chi tiết nhiều vai trò.
- Phần phối hợp thống kê có BoLocThoiGian dùng lại; DTO bộ lọc và BUS chuyển khoảng ngày/tháng/quý/năm trước khi gọi DAO. Còn lọc KH/NV/SP, tổ hợp và các nghiệp vụ thống kê thuộc phạm vi phối hợp.
- Ảnh form hiện có là bằng chứng ở giai đoạn trước; cần chụp lại và nghiệm thu tương tác theo bản hiện tại trước khi nộp báo cáo cuối.

## Kiểm chứng đợt đối chiếu mẫu 17/11/2026

- Build solution: 0 lỗi, 0 cảnh báo. Toàn bộ 123 checks tự động đạt, gồm kiểm tra tra cứu mã qua danh sách BUS, không gọi DAO đọc từng mã, chuẩn hóa mã, cache sau tải lại và cấu hình kết nối dùng chung. Các kiểm tra cache/ghi lỗi dùng DAO giả lập.
- Kiểm tra chỉ đọc MySQL thật qua MyConnection: bộ mã kết nối utf8mb4, đọc được 10 khách hàng, 10 nhân viên và 10 nhà cung cấp qua ba DAO đã chỉnh. Không chạy INSERT/UPDATE/DELETE hoặc sửa schema.
- Đợt đối chiếu ban đầu chỉ đọc CSDL; việc kiểm tra ghi được bổ sung ở mục dưới. Không coi số checks đạt là bằng chứng hoàn thành toàn bộ đồ án.

## Kiểm chứng bổ sung danh mục/ràng buộc — 17/11/2026

- SmartShop.MySqlChecks: 54 checks tích hợp MySQL thật đạt trên database riêng, dựng 19 bảng từ DDL gốc. Bao gồm CRUD ba danh mục, tiếng Việt/dấu nháy, lưu không đổi nội dung, FK chặn xóa, UNIQUE email KH/NV khi INSERT/UPDATE từ cache cũ, khóa mã và giữ cache khi lỗi.
- Đã thử hai BUS khách hàng cùng thêm một mã bằng hai tác vụ song song; đúng một lần ghi thành công. Có kiểm tra trùng mã NV/NCC qua hai phiên, tải lại đồng bộ dữ liệu và sửa NCC đã bị phiên khác xóa.
- Database kiểm thử được dọn trong finally và kiểm tra lại là không còn trong information_schema. Không ghi vào database ứng dụng, không chạy migration kho hay phần DROP/USE/dữ liệu mẫu trong script gốc.
- Các lỗi ghi là kiểm tra tính nguyên vẹn của từng câu lệnh, không nghiệm thu transaction nhiều bước, tải lớn, mọi kiểu tranh chấp hoặc giao diện web/mobile. Bộ kiểm tra nằm ngoài ứng dụng và không thay đổi sườn dự án.
