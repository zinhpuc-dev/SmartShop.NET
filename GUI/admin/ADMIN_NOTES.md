# Admin SmartShop

- Dang nhap tai /admin/login bang tai khoan Admin da thong nhat. Mat khau duoc bam,
  khong can tao bang tai khoan hay chay SQL de dang nhap.
- Co the ghi de Admin:Username va Admin:PasswordHash qua User Secrets cua host.
  Khong ghi mat khau thuan vao ma nguon. Khoi dong lai ung dung se ket thuc cac phien Admin.
- Cac trang Admin duoc bao ve phia server, dang xuat bang POST co chong CSRF.
- Giu nguyen luong User hien tai; khong doi DemoShopDao hoac checkout User trong dot nay.
- Phan bo kho cua don hang van can migration da duyet. Khong chay lai schema goc.
- Nguoi dung da cho phep kiem tra lai 08/11/2026: build, HTTP dang nhap va DAO chi doc da qua; khong kiem thu ghi MySQL.
