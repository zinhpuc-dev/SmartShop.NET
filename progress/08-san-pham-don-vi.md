# Sản phẩm, Đơn vị, tồn kho

**Người phụ trách:** Bảo  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | Phúc (khung BUS) |
| Yêu cầu / Use Case | UC05, UC07, UC20, UC24 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/sanpham` |
| Mục báo cáo | III (Bảo) |
| Ảnh form | `docs/images/08-san-pham-don-vi/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

ProductManageService/ProductManageDAO có thêm/sửa/ngừng bán, tìm kiếm, mã loại/trạng thái và tồn theo kho. Đã đọc 10 sản phẩm; tồn không nhập tay. DonViTinh vẫn là trường chuỗi; chưa có CRUD đơn vị/ảnh/ngưỡng tồn/tên danh mục.

### Mốc triển khai và kiểm chứng

- [x] Đọc 10 sản phẩm và tồn kho thật.
- [x] Có mã CRUD/ngừng bán, quản lý mã loại và validation giá.
- [ ] Kiểm thử ghi; hoàn thiện đơn vị/ảnh/ngưỡng tồn theo phạm vi nghiệm thu.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] CRUD Sản phẩm: mã, tên, giá nhập, giá bán, tồn, ảnh, danh mục, đơn vị
- [ ] CRUD Đơn vị; ngừng kinh doanh
- [ ] Danh sách và chi tiết sản phẩm
- [ ] Xem tồn kho, ngưỡng tồn tối thiểu đọc từ bảng tham số
- [ ] Tồn kho là field không được nhập tay
- [ ] Chụp ảnh form cho báo cáo
- [ ] Đã có ảnh form trong `docs/images/08-san-pham-don-vi/`
- [ ] Đã viết phần báo cáo tương ứng (III (Bảo))

## Tiêu chí nghiệm thu

Thêm, sửa, xóa, tìm được; tồn kho đổi đúng khi bán/nhập

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
