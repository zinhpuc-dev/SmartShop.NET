# Giải thích phần của Triết khi bảo vệ

Tài liệu ôn phần 4, 5, 6, 7, 9, 10, 15 và phần phối hợp 14, 16. Đây là ghi chú học và đối chiếu code, không phải nhật ký tiến độ hay xác nhận nghiệm thu toàn bộ đồ án.

## Trình bày trong 10 phút

| Thời gian | Nội dung cần nói | Mã nguồn để minh họa |
|---|---|---|
| 0–2 phút | GUI nhận dữ liệu, BUS xử lý nghiệp vụ, DAO thực hiện SQL, DTO truyền dữ liệu | QuanLyKhachHang, KhachHangBUS, KhachHangDAO, KhachHangDTO |
| 2–4 phút | ArrayList tại BUS, nạp một lần, tìm mã/tên trong bộ nhớ, tải lại khi cần | LayDanhSachKhachHang, TimKiem, LayTheoMa, TaiLai |
| 4–6 phút | Thêm/sửa/xóa; ghi DAO thành công mới đổi cache | Them, Sua, Xoa trong ba BUS danh mục |
| 6–8 phút | AND trong nhóm, OR giữa nhóm, NOT từng điều kiện; kiểm tra kiểu trường | TimKiemDanhMucNangCao và BoLocDanhMuc |
| 8–9 phút | Trùng mã/email, khóa mã khi sửa, FK chặn xóa và lỗi khi hai phiên thao tác | KiemTraTrung, KiemTraDuLieu và catch MySqlException tại DAO |
| 9–10 phút | Tái sử dụng và khác biệt giữa mẫu desktop với bản web/mobile | Component dùng chung, interface, DI scoped và MyConnection |

## Luồng ba lớp

Ví dụ thêm khách hàng: FormKhachHang ghi vào DuLieuKhachHang; trang QuanLyKhachHang gọi IKhachHangBUS.Them. KhachHangBUS chuẩn hóa và kiểm tra dữ liệu, kiểm tra trùng trong danh sách, tạo KhachHangDTO rồi gọi IKhachHangDAO.Them. KhachHangDAO dùng MyConnection mở kết nối, thực hiện INSERT có tham số và trả DTO khi thành công. BUS mới thêm kết quả vào ArrayList; GUI hiển thị thông báo.

DTO và lớp dữ liệu form nằm trong project DTO. BUS chỉ tham chiếu hợp đồng DAO, không biết implementation MySQL cụ thể. KhoiDong.cs ghép interface với implementation bằng DI; việc ghép tại điểm khởi động không phải màn hình tự truy cập SQL.

## ArrayList và tìm kiếm

`private ArrayList? customers` là dữ liệu đã nạp của một BUS. Lần đầu đọc, BUS gọi DAO.DocDanhSach và tạo ArrayList. Các lần TimKiem/LayTheoMa sau đó sử dụng danh sách này.

Mã rút gọn để viết trên giấy:

```csharp
public KhachHangDTO LayTheoMa(string code)
{
    string ma = (code ?? "").Trim();
    foreach (KhachHangDTO value in LayDanhSachKhachHang())
    {
        if (value.Code.Equals(ma, StringComparison.OrdinalIgnoreCase))
            return value;
    }
    throw new InvalidOperationException("Không tìm thấy khách hàng.");
}
```

LayDanhSachKhachHang trả một chuỗi DTO được duyệt từ ArrayList. LINQ trong tìm kiếm đa trường cũng duyệt dữ liệu bộ nhớ; không có IQueryable hay câu SQL tìm kiếm phát sinh khi gõ. Chuỗi được chuẩn hóa tiếng Việt không dấu để tìm tên thuận tiện.

TaiLai đặt cache về null; lần đọc kế tiếp mới nạp lại DAO. Danh sách rỗng vẫn là dữ liệu đã nạp, không phải cứ rỗng là truy vấn lại. Thay đổi từ người dùng khác chỉ xuất hiện sau khi tải lại.

## Vì sao ghi DAO trước khi sửa ArrayList?

Nếu thêm vào cache trước rồi INSERT bị trùng mã, giao diện sẽ hiện hồ sơ không có trong CSDL. Do đó:

- Thêm: kiểm tra → tạo DTO → DAO.Them → ArrayList.Add.
- Sửa: lấy hồ sơ → IndexOf → kiểm tra dữ liệu và khóa mã → tạo DTO mới → DAO.Sua → thay phần tử tại vị trí đã tìm.
- Xóa: tìm hồ sơ → DAO.Xoa → ArrayList.Remove.

Khi DAO ném lỗi, dòng cập nhật cache phía sau không chạy. DTO dạng record không bị sửa tại chỗ; form dùng bản sao để hủy nhập không làm thay đổi dữ liệu đang hiển thị.

## Ví dụ tìm kiếm nâng cao

Điều kiện: `(Giá bán > 10000000 AND Tồn kho <= 5 AND NOT Trạng thái = Ngừng bán) OR (Mã loại = L00002)`.

Nhóm 1 cần thỏa cả ba điều kiện. Nhóm 2 có một điều kiện mã loại. Chỉ cần một nhóm đúng thì lấy sản phẩm. NOT đảo kết quả của điều kiện được đánh dấu, không đảo cả nhóm.

Trường số hỗ trợ =, <>, >, >=, <, <=; trường văn bản hỗ trợ Chứa, =, <>. BUS từ chối toán tử không phù hợp, giá trị số sai, trường không tồn tại và nhóm rỗng trước khi lọc, kể cả danh sách dữ liệu đang rỗng. Giao diện chỉ áp dụng khi bấm nút; không hỗ trợ biểu thức tự do hoặc ngoặc lồng tùy ý.

## Ràng buộc và hai phiên

Kiểm tra tại form giúp báo lỗi sớm; kiểm tra tại BUS ngăn gọi trực tiếp bằng dữ liệu sai. Readonly ở ô mã chỉ là một phần: BUS còn đối chiếu mã trước khi sửa. Mã trên form thêm hiện là mã gợi ý, không phải bộ cấp mã tự động an toàn giữa mọi phiên.

BUS kiểm tra trùng dựa vào cache nên có thể chưa thấy dữ liệu vừa được phiên khác ghi. PRIMARY KEY/UNIQUE tại MySQL là lớp kiểm tra cuối. DAO chuyển lỗi 1062 thành thông báo mã/email đã dùng. FK chuyển lỗi 1451 thành thông báo không được xóa hồ sơ có liên kết. NCC hiện không có UNIQUE email trong schema, không mô tả rằng email NCC được khóa duy nhất.

Hai phiên cùng thêm một mã có thể đều vượt qua kiểm tra cache, nhưng chỉ một INSERT thành công. Phiên bị từ chối phải giữ cache cũ và tải lại nếu cần. Điều này không có nghĩa mọi loại xung đột sửa đồng thời đã được giải quyết; chưa có cơ chế khóa phiên bản để ngăn ghi đè thay đổi mới nhất.

## OOP, tái sử dụng và web/mobile

- Đóng gói: cache và kết nối nằm trong lớp, GUI gọi hành vi qua interface.
- Thay thế implementation: BUS nhận hợp đồng DAO; kiểm tra thường dùng DAO giả lập, kiểm tra tích hợp dùng DAO MySQL thật.
- Tái sử dụng GUI: BangDuLieu, BieuMauKiemTra, TruongNhapLieu, HopThoai và các component chọn hồ sơ. Không cần sao chép toàn bộ form để dùng popup trong nghiệp vụ khác.
- Callback: popup trả DTO về form theo EventCallback, không dùng biến static chung mọi người truy cập web.
- MyConnection: dùng chung cách tạo/mở kết nối; mỗi thao tác sở hữu một kết nối và giải phóng bằng using. Không giữ một kết nối mở static cho mọi phiên.
- Phần 4: layout và vùng ChildContent thay MDI/panel WinForms; tên tài khoản lấy từ phiên đăng nhập, menu/nội dung theo policy Admin.
- Phần phối hợp thống kê: DTO bộ lọc thời gian → BUS chuyển thành khoảng bắt đầu bao gồm/kết thúc loại trừ → DAO tổng hợp. Cận trên đầu ngày kế tiếp giúp lấy đủ ngày cuối kỳ.

## Bằng chứng và giới hạn

SmokeTests kiểm tra nghiệp vụ bằng DAO giả lập, không chứng minh MySQL ghi đúng. SmartShop.MySqlChecks tạo database kiểm thử riêng từ DDL gốc để kiểm tra CRUD, UNIQUE, FK và một số tình huống hai phiên. Đọc README trong thư mục bộ kiểm tra trước khi chạy.

Không dùng kết quả build hoặc test nghiệp vụ thay cho nghiệm thu nút bấm, bố cục desktop/mobile, ảnh form, đăng nhập nhiều tài khoản hoặc toàn bộ nghiệp vụ của thành viên khác. Khi trình bày cần nói đúng phần đã làm, phần phối hợp và phần còn chờ kiểm chứng.
