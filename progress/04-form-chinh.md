# Form chính (menu trái, form con)

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 12/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Cả nhóm gắn form vào menu |
| Yêu cầu / Use Case | YC13 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/04-form-chinh/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 12/11/2026

Menu Admin có tổng quan/sản phẩm/đơn/thống kê, khách hàng, nhân viên, NCC, kho và khuyến mại; đã bảo vệ route và thêm đăng xuất. KhungQuanTri hiển thị tên từ AuthenticationState và dùng AuthorizeView với policy SmartShopAdminOnly hiện có để kiểm soát cả menu lẫn vùng nội dung. Khi phiên không còn quyền Admin, giao diện quản trị được gỡ và hiện liên kết đăng nhập lại/về cửa hàng. Không thay đổi cơ chế đăng nhập hoặc tạo thêm vai trò mới.

Đã bổ sung đánh dấu mục đang mở cho các mục menu mobile còn thiếu, chỉnh bố cục thanh tiêu đề cho tên tài khoản và các nút trên điện thoại. Giữ nguyên sườn GUI/BUS/DAO/DTO, không sửa luồng User. Solution hiện vẫn nhiều project, chưa gom về một project.

### Mốc triển khai và kiểm chứng

- [x] Các route chính đã trả dữ liệu qua HTTP sau đăng nhập.
- [x] Có đăng xuất và kiểm soát truy cập Admin phía server.
- [x] Hiển thị danh tính theo phiên và kiểm soát menu/nội dung bằng policy Admin hiện có.
- [x] Build solution 0 lỗi/0 cảnh báo; toàn bộ 104 checks đạt, gồm 6 kiểm tra mới render KhungQuanTri trong bộ nhớ: ẩn danh, tài khoản thiếu quyền, Admin hợp lệ, đánh dấu menu desktop/mobile, đổi trạng thái sang hết phiên và HTML encode tên tài khoản.
- [ ] Kiểm tra trực tiếp thanh tiêu đề/menu sau thay đổi trên trình duyệt. Các checks render dùng AuthenticationStateProvider giả lập và policy thật, không thay thế kiểm thử cookie, thời gian hết hạn hay giao diện tương tác.
- [ ] Chụp ảnh mới, nghiệm thu tương tác desktop/mobile; chốt riêng việc gom project.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [x] Form chính: menu trái, vùng chứa form con (bản web dùng layout và vùng nội dung thay MDI/panel)
- [x] Thanh tiêu đề, hiển thị người dùng đang đăng nhập (lấy tên từ phiên; đã kiểm tra render)
- [x] Gắn các form con vào menu theo từng giai đoạn (các trang hiện có; chưa đủ toàn bộ chức năng đồ án)
- [x] Ẩn/hiện menu theo quyền (dùng policy Admin hiện có; chưa có phân quyền chi tiết từng chức năng)
- [x] Đã có ảnh form trong `docs/images/04-form-chinh/`
- [ ] Đã viết phần báo cáo tương ứng (III (Triết))

### Phạm vi trước 12/11/2026 (lịch sử)

- Bản web/mobile theo lựa chọn của người dùng: MainLayout/ShopShell và AdminLayout/AdminShell dùng lại bố cục chung; vùng `@Body`/`ChildContent` hiển thị trang chức năng tương ứng, thay cách nhúng form con MDI/panel của desktop.
- Có thanh tiêu đề, logo, breadcrumb, menu trái, mục menu đang chọn và thanh điều hướng dưới trên điện thoại. Thanh tiêu đề chưa hiển thị người dùng đăng nhập thật nên chưa tick trọn mục này.
- Menu User đã gắn trang chủ, sản phẩm, danh mục, đơn hàng, yêu thích, giỏ hàng, tài khoản, hỗ trợ và lối vào Admin demo.
- Menu Admin đã gắn tổng quan, khách hàng (`/admin/customers`), nhân viên (`/admin/employees`) và lối về cửa hàng. Các chức năng quản trị khác sẽ gắn theo tiến độ của nhóm.
- Router nạp thêm assembly Admin; các trang quản trị dùng AdminLayout riêng. ShopPageBase dùng chung điều hướng, nhận thay đổi đường dẫn và cập nhật UI; ShopOverlays dùng chung cho các bố cục.
- Đã có ảnh desktop/mobile: `docs/images/04-form-chinh/web-desktop.png` và `web-mobile.png`. Ảnh này ghi nhận giao diện ở giai đoạn trước; ảnh Admin khách hàng/nhân viên mới nằm ở thư mục phần 05/06, chưa thay ảnh phần 04.
- Đã đối chiếu code layout, menu, router, tên trang và trang tài khoản. Kiểm tra trình duyệt ở phần 06 đã xác nhận chuyển qua lại Khách hàng/Nhân viên; bố cục mobile 390 × 844 không tràn ngang. Lần build gần nhất không có lỗi/cảnh báo, 77 kiểm tra nghiệp vụ đạt; các kiểm tra nghiệp vụ này không thay thế kiểm tra phân quyền.
- Chưa có phần báo cáo Form chính hoàn chỉnh trong `docs/bao-cao/III-triet.md`.

## Tiêu chí nghiệm thu

Form chính mở được đủ các form con theo mẫu giảng viên

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 12/11/2026 | Triết | Hiển thị tên tài khoản từ phiên; dùng AuthorizeView/policy Admin để ẩn menu và nội dung khi không đủ quyền; thêm lối đăng nhập lại, đánh dấu mục menu mobile và chỉnh thanh tiêu đề | Codex | Build 0 lỗi/0 cảnh báo; 104 checks đạt, gồm 6 checks render khung với phiên giả lập. Chưa nghiệm thu trực tiếp trình duyệt, cookie hết hạn hay ảnh mới; còn báo cáo và tích hợp đủ chức năng |
| 03/11/2026 | Triết | Rà soát phần 04 đã có: layout User/Admin riêng, menu trái, vùng nội dung, breadcrumb, điều hướng mobile và liên kết các trang đang triển khai; xác nhận đã có ảnh desktop/mobile | Codex | Đang làm, 3/6 mục. Còn tên người đăng nhập, phân quyền menu, tích hợp đủ trang và báo cáo chính. Chỉ cập nhật nhật ký, chưa commit/push |

| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

- Hai vướng mắc lịch sử ngày 03/11/2026 về danh tính và quyền truy cập đã có triển khai: route được bảo vệ, thanh tiêu đề/menu dùng phiên và policy Admin. Còn nghiệm thu trực tiếp trạng thái cookie/phiên hết hạn và giao diện desktop/mobile sau thay đổi.
- Hệ thống hiện có một tài khoản quản trị cục bộ; phân quyền chi tiết nhiều vai trò/từng chức năng cần phối hợp Phúc, không coi thay đổi hiển thị menu là đã hoàn thành phạm vi đó.
- Chưa mở đủ các chức năng theo toàn bộ mẫu giảng viên vì một số module chưa triển khai; cần gắn menu khi các thành viên bàn giao.
- Bản hiện tại là web/mobile, chưa triển khai MDI/panel WinForms; cần đối chiếu với giảng viên về việc áp dụng yêu cầu form chính cho nền tảng web.
- Chưa viết/gộp báo cáo Form chính vào mục III chính.
