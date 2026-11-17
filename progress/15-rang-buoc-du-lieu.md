# Ràng buộc dữ liệu: duy nhất, field bị khóa

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 17/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Vinh (UNIQUE trong CSDL), cả nhóm |
| Yêu cầu / Use Case | YC5, YC7 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/15-rang-buoc-du-lieu/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 17/11/2026

Bổ sung tools/SmartShop.MySqlChecks và README hướng dẫn chạy bộ kiểm tra MySQL tích hợp trên database ngẫu nhiên riêng. Dựng 19 bảng từ DDL gốc, không chạy USE/DROP/dữ liệu mẫu hoặc migration kho; chỉ ghi vào CSDL thử. Cuối chạy xóa đúng database do lần chạy tạo và kiểm tra lại không còn trong information_schema.

- 54 checks MySQL đạt: CRUD ba danh mục, dữ liệu sai, mã bị khóa, lỗi UNIQUE email KH/NV khi INSERT/UPDATE với cache cũ, giữ email của chính bản ghi, FK giỏ/đơn/hóa đơn/phiếu nhập chặn xóa, trùng mã từ phiên khác và giữ nguyên cache khi lỗi.
- Đã thử hai BUS KH cùng thêm một mã bằng hai tác vụ song song: đúng một INSERT thành công; phiên thua không thêm cache. Có kiểm tra NV/NCC trùng mã qua hai phiên, tải lại đồng bộ cache, NCC bị phiên khác xóa trước khi sửa.
- Đối chiếu schema gốc: KH/NV có UNIQUE email, ba danh mục có PRIMARY KEY; NCC không có UNIQUE email. Không thay đổi ràng buộc CSDL trong đợt này.
- 123 smoke checks hiện có đạt. Build bộ kiểm tra và solution sau dọn file: 0 lỗi/0 cảnh báo. Đã bổ sung mục ràng buộc và bằng chứng kiểm tra vào nháp báo cáo III, tài liệu giải thích vào docs/HUONG_DAN_BAO_VE_TRIET.md.
- Các lỗi ghi được kiểm tra ở mức câu lệnh, không phải rollback transaction nhiều bước hoặc nghiệm thu mọi kiểu sửa đồng thời. Còn nghiệm thu UI/ảnh, rà toàn bộ trường tự sinh/tính toán của các module phối hợp và review báo cáo cuối.

## Mốc trước — 10/11/2026

Phần ràng buộc đang làm, hoàn thành 2/6 mục trong checklist chính ở mức triển khai code. Việc nghiệm thu ghi MySQL, dữ liệu trùng và đồng thời nhiều phiên vẫn còn mở.

### Phạm vi bổ sung và kiểm chứng

- CustomerBUS và EmployeeBUS kiểm tra trùng email đã chuẩn hóa, không phân biệt hoa/thường khi thêm/sửa; cho phép giữ email của chính bản ghi và email rỗng. Tiếp tục dùng kiểm tra mã trùng, trường bắt buộc và khóa mã tại BUS hiện có.
- CustomerDAO và EmployeeDAO bắt lỗi MySQL 1062 khi sửa, trả thông báo mã/email đã được sử dụng. Xử lý này bổ sung cho trường hợp ràng buộc UNIQUE phát hiện dữ liệu trùng từ phiên khác.
- Đã bổ sung kiểm tra tự động cho email trùng khi thêm/sửa, giữ nguyên dữ liệu khi từ chối, giữ email của chính bản ghi, lỗi ghi không làm đổi cache và chặn sửa mã khách hàng/nhân viên/nhà cung cấp tại BUS.
- Build thành công, 0 lỗi/0 cảnh báo. Toàn bộ 78 smoke checks đạt, trong đó 11 kiểm tra ràng buộc mới dùng repository giả lập.
- Chưa kiểm thử trực tiếp lỗi UNIQUE khi UPDATE MySQL, rollback hoặc tranh chấp giữa nhiều phiên. Các mục UNIQUE phối hợp Vinh và khóa toàn bộ trường tính toán/tự sinh vẫn để mở để nghiệm thu đầy đủ.
- Chưa có ảnh riêng phần 15 và chưa viết phần báo cáo tương ứng.

## Mốc trước — 08/11/2026

BUS kiểm tra đầu vào/mã/giá, DAO SQL có tham số/FK; sửa validation voucher theo chương trình/thời gian/số lượt dùng, chuẩn hóa PhanTram/SoTien. Chuyển trạng thái đơn và transaction có trong code; chưa thử ghi/concurrency.

### Mốc triển khai và kiểm chứng

- [x] Có validation và SQL tham số trong các module Admin.
- [x] Giao diện phân biệt tải/rỗng/lỗi, thông báo thân thiện.
- [ ] Kiểm thử ràng buộc ghi, rollback, dữ liệu trùng và concurrency.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [x] Kiểm tra duy nhất trong code; đã kiểm chứng mã KH/NV/NCC và email KH/NV trên MySQL riêng (module khác còn cần đối chiếu)
- [ ] UNIQUE trong CSDL (Vinh) và kiểm tra ở BUS (Triết): đã kiểm chứng KH/NV/NCC; còn rà phạm vi các module khác
- [ ] Field không được nhập: mã tự sinh, thành tiền, tồn kho
- [x] Thông báo lỗi rõ ràng cho người dùng (bổ sung thông báo trùng mã/email khi sửa KH/NV)
- [ ] Đã có ảnh form trong `docs/images/15-rang-buoc-du-lieu/`
- [x] Đã viết phần báo cáo tương ứng (bản nháp III-triet.md đã có nội dung; còn review, ảnh mới và chốt bản cuối)

## Tiêu chí nghiệm thu

Không trùng mã; field khóa không sửa được

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 17/11/2026 | Triết | Bổ sung 54 checks MySQL thật cho CRUD, UNIQUE/FK, khóa mã và cache giữa hai phiên trên DB thử tự dọn; bổ sung báo cáo/tài liệu | Codex | 54 MySQL checks và 123 smoke checks đạt; không đổi DB ứng dụng. Chưa nghiệm thu transaction nhiều bước, tải lớn, UI hoặc mọi kiểu tranh chấp |
| 10/11/2026 | Triết | Bổ sung chặn email trùng tại BUS khi thêm/sửa KH/NV, bắt lỗi UNIQUE khi sửa trong DAO và kiểm tra khóa mã/cache khi ghi lỗi | Codex | Đang làm, 2/6 mục checklist chính. 11 checks ràng buộc mới qua với repository giả lập, 78 checks toàn bộ đạt. Chờ nghiệm thu ghi MySQL, ảnh và báo cáo; local develop, chưa commit/push |
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

- Phối hợp Vinh kiểm chứng UNIQUE/FK trên CSDL, đặc biệt lỗi dữ liệu trùng khi UPDATE và tranh chấp giữa nhiều phiên.
- Cần kiểm thử ghi/rollback trên database dành riêng cho kiểm thử trước khi nghiệm thu đầy đủ.
- Rà đủ trường mã tự sinh, thành tiền và tồn kho theo toàn bộ nghiệp vụ để chốt mục khóa trường.
- Cần chụp ảnh phần 15 và viết báo cáo mục III.
