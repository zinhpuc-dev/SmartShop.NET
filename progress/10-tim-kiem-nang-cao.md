# Tìm kiếm nâng cao (and, or, not, so sánh)

**Người phụ trách:** Triết  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 10/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Phúc (review BUS) |
| Yêu cầu / Use Case | YC8 |
| Mức ưu tiên | 1 |
| Nhánh Git | `develop` (local, chưa commit/push) |
| Mục báo cáo | III (Triết) |
| Ảnh form | `docs/images/10-tim-kiem-nang-cao/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Việc cần làm

- [x] Kết hợp and, or, not trên một hoặc nhiều field
- [x] Các phép so sánh >, >=, <, <=, <>
- [x] Ví dụ: giá > X and tồn <= Y and not hết hàng
- [x] Giao diện nhập điều kiện
- [ ] Phúc review BUS
- [x] Đã có ảnh form trong `docs/images/10-tim-kiem-nang-cao/`
- [ ] Đã viết phần báo cáo tương ứng (III (Triết))

### Phạm vi đã thực hiện

- Hoàn thành 5/7 mục trong checklist chính. Phần review BUS của Phúc và báo cáo mục III còn chờ thực hiện.
- DTO CatalogFilter/SearchGroup/SearchCondition biểu diễn các nhóm OR, các điều kiện AND trong nhóm và cờ NOT cho từng điều kiện.
- AdvancedCatalogSearch tại BUS dùng chung cho Sản phẩm, Khách hàng, Nhân viên và Nhà cung cấp; lọc trên danh sách ArrayList đã nạp, kết hợp được với tìm kiếm cơ bản và loại sản phẩm.
- Trường số hỗ trợ =, <>, >, >=, <, <=. Trường văn bản hỗ trợ Chứa, =, <> với chuẩn hóa tiếng Việt không dấu. BUS chặn trường/phép so sánh sai, giá trị số không hợp lệ và nhóm điều kiện rỗng.
- Component AdvancedSearch dùng lại ở cả bốn trang Admin; cho thêm/xóa điều kiện, thêm/xóa nhóm và bỏ bộ lọc. Giới hạn 8 nhóm, mỗi nhóm tối đa 8 điều kiện. Điều kiện đang sửa chỉ áp dụng khi người dùng bấm nút; nhập sai giữ nguyên bộ lọc đã áp dụng.
- Đã thử trên trình duyệt với dữ liệu MySQL của cả bốn danh mục. Biểu thức (giá > 10.000.000 AND tồn <= 30 AND NOT tồn = 0) OR tên chứa Keychron trả đúng iPhone, Samsung Galaxy và bàn phím Keychron trong dữ liệu mẫu.
- Đã thử nhập số sai và bỏ bộ lọc; giao diện mobile 390 × 844 không tràn ngang. Có 2 ảnh trong `docs/images/10-tim-kiem-nang-cao/`, gồm `01-ket-hop-and-or-not.png` và `02-mobile-bo-loc.png`.
- Build thành công, 0 lỗi/0 cảnh báo. Toàn bộ 78 smoke checks đạt, gồm 40 kiểm tra trước đó, 27 kiểm tra tìm kiếm nâng cao và 11 kiểm tra ràng buộc bổ sung. Các kiểm tra tự động dùng repository giả lập, không ghi vào MySQL thật.
- Chú thích trong phần tìm kiếm mới và các file thuộc phần Triết được rà soát, chuyển sang tiếng Việt.

## Tiêu chí nghiệm thu

Tìm kiếm nâng cao dùng được đủ and, or, not và các phép so sánh

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 10/11/2026 | Triết | Triển khai bộ lọc AND/OR/NOT và các phép so sánh tại BUS, component nhập điều kiện dùng chung cho bốn danh mục; thử MySQL, desktop/mobile và lưu ảnh | Codex | Đang làm, 5/7 mục. Build đạt, 78 checks toàn bộ qua. Chờ Phúc review BUS và báo cáo III; local develop, chưa commit/push |

## Vướng mắc

- Chờ Phúc review BUS và đối chiếu với yêu cầu giảng viên.
- Cần viết phần báo cáo III kèm giải thích nhóm AND/OR, NOT và các phép so sánh.
- Giao diện hiện biểu diễn nhóm AND nối bằng OR, NOT trên từng điều kiện. Chưa có ô nhập biểu thức tự do hoặc dấu ngoặc lồng tùy ý.
