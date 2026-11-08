# SmartShop Admin — implementation and verification report

## Current status

Admin workflows are implemented in the .NET 10 Blazor application with GUI → BUS → DTO contracts → DAO MySQL persistence. `dotnet build SmartShop.slnx` passed, and the smoke suite passed after BUS normalization was corrected before DataAnnotations validation. The suite includes warehouse/receiving, order/report, and promotion/voucher checks.

No destructive schema script or database write has been run. The Admin-auth/order-warehouse additive migration remains unapplied. Therefore, real database sign-in, actual MySQL writes, and order allocation using `DON_HANG.MaKho` are not verified.

## Module status

| Module | Implementation | Status / limits |
|---|---|---|
| Admin authentication | Role-protected routes, cookie login, password hashes, setup utility | Implemented. Requires the reviewed additive migration and account provisioning before real DB sign-in works. |
| Customers | MySQL CRUD for `KHACH_HANG` | Implemented; live writes not run. |
| Employees | MySQL CRUD for `NHAN_VIEN`, including `ChucVu` | Implemented; live writes not run. |
| Suppliers | MySQL CRUD for `NHA_CUNG_CAP` | Implemented; live writes not run. |
| Products/categories | Product CRUD, category code/status, soft-deactivation | Implemented. `LOAI_SAN_PHAM` has no category-name column. |
| Stock/receiving | `TON_KHO` totals and per-warehouse reads; warehouse management; receipts update header/details/stock in one transaction | Implemented in code; no live receipt was posted. |
| Orders | MySQL orders/details; transactional warehouse stock deduction on delivery completion | Implemented in code. Migration adding `DON_HANG.MaKho` is not applied. Existing orders without a warehouse need allocation before completing stock-affecting transitions. |
| Invoices/reports | MySQL invoice/detail reads and 30-day sales summary | Implemented; not verified against a live database. |
| Promotions/vouchers | MySQL CRUD UI/DAO with date, discount, and redeemed-quantity validation | Implemented; normalization and discount-rule smoke checks pass. |

## Data safety

- Connection lookup uses ASP.NET configuration `ConnectionStrings:SmartShop`, then `SMARTSHOP_DB_CONNECTION`. Keep credentials out of source control and chat.
- `Database/QuanLyCuaHangDienTu_MySQL.sql` is a reference schema and contains `DROP TABLE`; do not run it against the existing database.
- Migration awaiting review: `Database/migrations/2026-03-Admin-Auth-Order-Warehouse.sql`. It creates `TAI_KHOAN_ADMIN` and adds nullable `DON_HANG.MaKho`; it has not been applied.
- No writes or destructive testing were performed against the configured database.

## Build and tests

Build passed:

```powershell
dotnet build SmartShop.slnx
```

Run the console smoke suite:

```powershell
dotnet run --project tools/SmartShop.SmokeTests/SmartShop.SmokeTests.csproj
```

The smoke command passed after the promotion normalization fix. Smoke tests use fake/demo repositories; they do not prove live MySQL operations or account login.

## Run the web app

From the repository root in PowerShell:

```powershell
dotnet run --project GUI/user/SmartShop.User.csproj
```

The configured launch profiles use HTTP `http://localhost:5264` and HTTPS `https://localhost:7040` (HTTPS profile also binds HTTP on port 5264). Open the URL printed by the app. Admin routes include `/admin/login`, `/admin`, `/admin/customers`, `/admin/employees`, `/admin/suppliers`, `/admin/inventory`, and `/admin/promotions`. Admin pages require an authenticated Admin account. Configure `ConnectionStrings:SmartShop` in local User Secrets (ID `SmartShop.Admin.Local`) or set `SMARTSHOP_DB_CONNECTION`. Never commit credentials. Back up the database and review the additive migration before applying it or provisioning an Admin account.

## Remaining work

1. In an isolated test database, review/apply the additive migration, provision an Admin account, and verify login/authorization.
2. Test CRUD, receiving rollback, stock deduction/idempotence, invoice reads, and reports only with isolated or disposable data.
3. Verify browser flows against a configured test database. Do not use production or existing seed data for destructive tests.
