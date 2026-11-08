# Đăng ký, đăng nhập, phân quyền

**Người phụ trách:** Phúc  
**Trạng thái:** Đang làm
**Cập nhật lần cuối:** 08/11/2026

| Mục | Nội dung |
|---|---|
| Phối hợp | — |
| Yêu cầu / Use Case | UC01, UC02 |
| Mức ưu tiên | 1 |
| Nhánh Git | `feature/dangnhap` |
| Mục báo cáo | III (Phúc) |
| Ảnh form | `docs/images/03-dang-nhap-phan-quyen/` |

Trạng thái dùng một trong: Chưa làm, Đang làm, Chờ review, Xong, Bị chặn.

## Cập nhật hiện tại — 08/11/2026

Đã khôi phục một Admin cục bộ: PasswordHasher, cookie HttpOnly, CSRF, giới hạn đăng nhập, session và đăng xuất phía server. Không cần bảng tài khoản; không ghi mật khẩu trong tài liệu. Chưa có đăng ký hoặc phân quyền nhân viên.

### Mốc triển khai và kiểm chứng

- [x] HTTP đăng nhập đúng/sai và từ chối thiếu CSRF đạt.
- [x] Truy cập ẩn danh bị chuyển về login; đăng xuất xong bị chặn.
- [ ] Nghiệm thu quyền nhân viên/đăng ký nếu yêu cầu này còn áp dụng.

> Các mô tả và số test ngày cũ bên dưới là lịch sử. Không đánh dấu hoàn tất các yêu cầu rộng hơn phần đã làm/kiểm chứng.

## Việc cần làm

- [ ] Form đăng ký tài khoản, kiểm tra trùng
- [x] Form đăng nhập Admin và băm mật khẩu (HTTP đã kiểm chứng)
- [ ] Phân quyền Admin/Nhân viên
- [x] Cấu hình một Admin cục bộ bằng mật khẩu băm; không công bố mật khẩu
- [ ] Chụp ảnh form có chú thích cho báo cáo
- [ ] Đã có ảnh form trong `docs/images/03-dang-nhap-phan-quyen/`
- [ ] Đã viết phần báo cáo tương ứng (III (Phúc))

## Tiêu chí nghiệm thu

Đăng nhập đúng quyền; mật khẩu không lưu dạng thô; có tài khoản demo

## Nhật ký tiến độ

| Ngày | Người | Đã làm | Công cụ AI | Ghi chú |
|---|---|---|---|---|
| 08/11/2026 | Codex (hỗ trợ nhóm) | Đồng bộ trạng thái Admin MySQL và bằng chứng kiểm tra | Codex | Chưa kiểm thử ghi, migration chưa chạy |

## Vướng mắc

(Ghi vấn đề đang chặn, người cần hỗ trợ, ngày phát hiện)
