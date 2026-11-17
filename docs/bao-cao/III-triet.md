# III. Các chức năng chính — Triết

Cập nhật 17/11/2026 theo mã nguồn hiện tại, đối chiếu mẫu c-3-layer. Giữ nền tảng Blazor web/mobile và phân công thành viên. Các thao tác ghi MySQL chưa được nghiệm thu đầy đủ; build và kiểm tra với DAO giả lập không thay thế kiểm thử ghi trên CSDL thật.

## Cấu trúc ba lớp và DTO

- GUI: các trang và component Razor nhận đầu vào, hiển thị dữ liệu/thông báo, gọi BUS qua interface; không viết SQL hoặc mở kết nối MySQL trong màn hình danh mục.
- BUS: KhachHangBUS, NhanVienBUS, NhaCungCapBUS giữ ArrayList theo phiên; kiểm tra dữ liệu hợp lệ, mã duy nhất, khóa mã khi sửa và xử lý tìm kiếm. Khách hàng/nhân viên còn kiểm tra trùng email không rỗng.
- DAO: KhachHangDAO, NhanVienDAO, NhaCungCapDAO thực hiện đọc/thêm/sửa/xóa bằng SQL có tham số. Ba DAO dùng lớp MyConnection chung để tạo/mở kết nối; dùng using để giải phóng sau mỗi thao tác. Chuỗi kết nối nhận từ cấu hình ngoài mã nguồn, mặc định utf8mb4 nếu chưa chỉ định bộ mã.
- DTO: KhachHangDTO, NhanVienDTO, NhaCungCapDTO truyền dữ liệu; các lớp DuLieuKhachHang/DuLieuNhanVien/DuLieuNhaCungCap chứa dữ liệu form và ràng buộc DataAnnotations. DTO không truy cập MySQL.
- GUI → BUS → hợp đồng DAO → DAO MySQL là luồng nghiệp vụ. KhoiDong.cs chỉ ghép các implementation qua DI. BUS không tham chiếu assembly DAO cụ thể.
- Tương ứng mẫu desktop: Razor/component thay form và ListView; dữ liệu BUS đóng gói theo phiên thay danh sách static dùng chung mọi người. ArrayList, tra cứu mã trong danh sách BUS và lớp MyConnection giữ cách phân chia trách nhiệm của mẫu.

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
6. **Xử lý:** NhaCungCapBUS nạp ArrayList qua NhaCungCapDAO; tra cứu mã và tìm đa trường trong BUS. Ghi thành công mới đổi cache; DAO chuyển lỗi khóa ngoại thành thông báo không thể xóa hồ sơ có phiếu nhập. Popup chọn NCC đã nối callback với SupplierCode của phiếu nhập. Đợt kiểm chứng trước đã đọc HTTP 10 hồ sơ; còn nghiệm thu ghi/xóa có liên kết, đồng thời nhiều phiên và tương tác popup trên trình duyệt.

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
- Chưa nghiệm thu CRUD MySQL, tranh chấp nhiều phiên hoặc tương tác web/mobile trong đợt này. Không coi số checks đạt là bằng chứng hoàn thành toàn bộ đồ án.
