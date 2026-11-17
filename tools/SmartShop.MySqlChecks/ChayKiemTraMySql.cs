using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO;

try
{
    await KiemTraMySql.Chay(args.Contains("--local-secrets", StringComparer.Ordinal));
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FAIL MYSQL {ex.GetType().Name}: {ex.Message}");
    return 1;
}

internal static class KiemTraMySql
{
    private static int soKiemTra;

    public static async Task Chay(bool dungSecretsCucBo)
    {
        string? chuoiQuanTri = Environment.GetEnvironmentVariable("SMARTSHOP_TEST_ADMIN_CONNECTION");
        if (string.IsNullOrWhiteSpace(chuoiQuanTri) && dungSecretsCucBo)
        {
            var cauHinh = new ConfigurationBuilder().AddUserSecrets("SmartShop.Admin.Local").Build();
            var goc = cauHinh.GetConnectionString("SmartShop")
                ?? Environment.GetEnvironmentVariable("SMARTSHOP_DB_CONNECTION");
            var matKhau = cauHinh["LocalMySQL:RootPassword"];
            if (string.IsNullOrWhiteSpace(goc) || string.IsNullOrWhiteSpace(matKhau))
                throw new InvalidOperationException("Thiếu cấu hình MySQL cục bộ cho bộ kiểm tra riêng.");
            chuoiQuanTri = new MySqlConnectionStringBuilder(goc) { UserID = "root", Password = matKhau }.ConnectionString;
        }
        if (string.IsNullOrWhiteSpace(chuoiQuanTri))
            throw new InvalidOperationException("Cần SMARTSHOP_TEST_ADMIN_CONNECTION hoặc --local-secrets. Bộ kiểm tra tạo database mới rồi tự dọn.");

        var cauHinhQuanTri = new MySqlConnectionStringBuilder(chuoiQuanTri)
        {
            Database = "", Pooling = false, CharacterSet = "utf8mb4"
        };
        var bangMau = DocCauTaoBang();
        var tenCSDL = "smartshop_test_triet_" + Guid.NewGuid().ToString("N");
        await using var quanTri = new MySqlConnection(cauHinhQuanTri.ConnectionString);
        await quanTri.OpenAsync();
        bool daTao = false;
        try
        {
            await using (var lenh = quanTri.CreateCommand())
            {
                lenh.CommandText = $"CREATE DATABASE `{tenCSDL}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
                await lenh.ExecuteNonQueryAsync();
                daTao = true;
            }
            Console.WriteLine("Database kiểm thử riêng: " + tenCSDL);
            var cauHinhThu = new MySqlConnectionStringBuilder(cauHinhQuanTri.ConnectionString)
            {
                Database = tenCSDL, UseAffectedRows = true
            };
            var chuoiThu = cauHinhThu.ConnectionString;
            foreach (var cauTao in bangMau) ThucThi(chuoiThu, cauTao);
            KiemTra(bangMau.Count == 19, "Dựng 19 bảng từ DDL gốc, không chạy USE/DROP/INSERT của script mẫu");

            var khDAO = new KhachHangDAO(chuoiThu);
            var nvDAO = new NhanVienDAO(chuoiThu);
            var nccDAO = new NhaCungCapDAO(chuoiThu);
            var khBUS = new KhachHangBUS(khDAO);
            var nvBUS = new NhanVienBUS(nvDAO);
            var nccBUS = new NhaCungCapBUS(nccDAO);
            KiemTraCrud("Khách hàng", "KH000001", TaoKhachHang, khBUS.Them, khBUS.Sua, khBUS.Xoa,
                khDAO.DocTheoMa, khBUS.LayTheoMa, value => value.Name);
            KiemTraCrud("Nhân viên", "NV000001", TaoNhanVien, nvBUS.Them, nvBUS.Sua, nvBUS.Xoa,
                nvDAO.DocTheoMa, nvBUS.LayTheoMa, value => value.Name);
            KiemTraCrud("Nhà cung cấp", "N00001", TaoNhaCungCap, nccBUS.Them, nccBUS.Sua, nccBUS.Xoa,
                nccDAO.DocTheoMa, nccBUS.LayTheoMa, value => value.Name);

            KiemTraLienKet(chuoiThu);
            KiemTraEmailKhachHang(chuoiThu);
            KiemTraEmailNhanVien(chuoiThu);
            await KiemTraHaiPhien(chuoiThu);
        }
        finally
        {
            // Chỉ xóa database tên ngẫu nhiên do chính lần chạy này tạo thành công.
            if (daTao && Regex.IsMatch(tenCSDL, "^smartshop_test_triet_[a-f0-9]{32}$") && quanTri.Database.Length == 0)
            {
                await using var lenh = quanTri.CreateCommand();
                lenh.CommandText = $"DROP DATABASE `{tenCSDL}`";
                await lenh.ExecuteNonQueryAsync();
                Console.WriteLine("Đã dọn database kiểm thử: " + tenCSDL);
                await using var kiemTra = quanTri.CreateCommand();
                kiemTra.CommandText = "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME=@Ten";
                kiemTra.Parameters.AddWithValue("@Ten", tenCSDL);
                KiemTra(Convert.ToInt32(await kiemTra.ExecuteScalarAsync()) == 0, "Database kiểm thử đã được xóa sau khi chạy");
            }
        }
        Console.WriteLine($"MYSQL CHECKS PASSED: {soKiemTra}");
    }

    private static List<string> DocCauTaoBang()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var path = Path.Combine(dir.FullName, "Database", "QuanLyCuaHangDienTu_MySQL.sql");
            if (!File.Exists(path)) continue;
            var bang = File.ReadAllLines(path).Where(line => Regex.IsMatch(line,
                "^CREATE TABLE `[A-Z_]+` \\(.*\\) ENGINE=InnoDB .*;$")).ToList();
            if (bang.Count != 19) throw new InvalidOperationException("DDL mẫu đã đổi; cần rà lại bộ kiểm tra trước khi tạo CSDL.");
            return bang;
        }
        throw new FileNotFoundException("Không tìm thấy schema gốc trong repository.");
    }

    private static void KiemTraCrud<T, TForm>(string nhan, string ma, Func<string, string, TForm> tao,
        Func<TForm, T> them, Func<string, TForm, T> sua, Action<string> xoa,
        Func<string, T?> doc, Func<string, T> traCache, Func<T, string> ten) where T : class
    {
        them(tao(ma, "  Nguyễn O'An – thử tiếng Việt  "));
        KiemTra(doc(ma) is { } daThem && ten(daThem) == "Nguyễn O'An – thử tiếng Việt",
            nhan + ": thêm, đọc lại tiếng Việt và dấu nháy qua SQL tham số");
        sua(ma, tao(ma, "Hồ sơ đã sửa"));
        KiemTra(doc(ma) is { } daSua && ten(daSua) == "Hồ sơ đã sửa" && ten(traCache(ma)) == "Hồ sơ đã sửa",
            nhan + ": sửa được MySQL và ArrayList");
        sua(ma, tao(ma, "Hồ sơ đã sửa"));
        KiemTra(doc(ma) is not null, nhan + ": lưu lại không đổi dữ liệu vẫn thành công với UseAffectedRows");
        TuChoi(() => sua(ma, tao(ma, "")), nhan + ": dữ liệu sai bị chặn trước khi ghi");
        KiemTra(doc(ma) is { } conNguyen && ten(conNguyen) == "Hồ sơ đã sửa" && ten(traCache(ma)) == "Hồ sơ đã sửa",
            nhan + ": lỗi đầu vào giữ nguyên CSDL và cache");
        xoa(ma);
        KiemTra(doc(ma) is null, nhan + ": xóa được hồ sơ không có liên kết");
        TuChoi(() => traCache(ma), nhan + ": xóa thành công gỡ hồ sơ khỏi ArrayList");
    }

    private static void KiemTraLienKet(string chuoi)
    {
        var khDAO = new KhachHangDAO(chuoi);
        var nvDAO = new NhanVienDAO(chuoi);
        var nccDAO = new NhaCungCapDAO(chuoi);
        var kh = new KhachHangBUS(khDAO);
        var nv = new NhanVienBUS(nvDAO);
        var ncc = new NhaCungCapBUS(nccDAO);
        kh.Them(TaoKhachHang("KH900001", "Khách có liên kết"));
        nv.Them(TaoNhanVien("NV900001", "Nhân viên có liên kết"));
        ncc.Them(TaoNhaCungCap("N90001", "NCC có liên kết"));
        ThucThi(chuoi, "INSERT INTO KHO VALUES ('K90001','Kho kiểm thử')");
        ThucThi(chuoi, "INSERT INTO GIO_HANG (MaGioHang,MaKH,NgayTao) VALUES ('GH900001','KH900001',NOW())");
        ThucThi(chuoi, "INSERT INTO DON_HANG VALUES ('DH900001','KH900001',NOW(),'Đã giao','Địa chỉ thử',100)");
        ThucThi(chuoi, "INSERT INTO HOA_DON VALUES ('HD900001','DH900001','NV900001',NOW(),100,'Tiền mặt','Đã thanh toán')");
        ThucThi(chuoi, "INSERT INTO PHIEU_NHAP VALUES ('PN900001','N90001','K90001',NOW())");
        TuChoi(() => kh.Xoa("KH900001"), "Chặn xóa KH có giỏ/đơn bằng lỗi FK thật", 1451);
        TuChoi(() => nv.Xoa("NV900001"), "Chặn xóa NV có hóa đơn bằng lỗi FK thật", 1451);
        TuChoi(() => ncc.Xoa("N90001"), "Chặn xóa NCC có phiếu nhập bằng lỗi FK thật", 1451);
        KiemTra(khDAO.DocTheoMa("KH900001") == kh.LayTheoMa("KH900001"), "Xóa KH lỗi giữ nguyên CSDL và cache");
        KiemTra(nvDAO.DocTheoMa("NV900001") == nv.LayTheoMa("NV900001"), "Xóa NV lỗi giữ nguyên CSDL và cache");
        KiemTra(nccDAO.DocTheoMa("N90001") == ncc.LayTheoMa("N90001"), "Xóa NCC lỗi giữ nguyên CSDL và cache");
        var nccCu = ncc.LayTheoMa("N90001");
        var doiMa = DuLieuNhaCungCap.TuDuLieu(nccCu); doiMa.Code = "N99999";
        TuChoi(() => ncc.Sua(nccCu.Code, doiMa), "Không thể đổi mã NCC tại BUS");
        KiemTra(nccDAO.DocTheoMa(nccCu.Code) == nccCu && ncc.LayTheoMa(nccCu.Code) == nccCu,
            "Đổi mã NCC bị từ chối giữ nguyên dữ liệu/cache");
    }

    private static void KiemTraEmailKhachHang(string chuoi)
    {
        var dao = new KhachHangDAO(chuoi);
        var bus = new KhachHangBUS(dao);
        var a = TaoKhachHang("KH700001", "Khách A"); a.Email = "a@example.test";
        var b = TaoKhachHang("KH700002", "Khách B"); b.Email = "b@example.test";
        bus.Them(a); var cu = bus.Them(b);
        dao.Sua(dao.DocTheoMa(a.Code)! with { Email = "changed@example.test" });
        b.Email = "CHANGED@example.test";
        TuChoi(() => bus.Sua(b.Code, b), "UNIQUE email KH khi UPDATE với cache cũ giữa hai phiên", 1062);
        KiemTra(dao.DocTheoMa(b.Code) == cu && bus.LayTheoMa(b.Code) == cu,
            "UPDATE KH bị UNIQUE từ chối không đổi dữ liệu/cache");
        var them = TaoKhachHang("KH700003", "Khách C"); them.Email = b.Email;
        TuChoi(() => bus.Them(them), "UNIQUE email KH khi INSERT với cache cũ", 1062);
        KiemTra(dao.DocTheoMa(them.Code) is null && bus.TimKiem(them.Code).Count == 0,
            "INSERT KH bị UNIQUE từ chối không thêm cache");
        var maSai = DuLieuKhachHang.TuDuLieu(cu); maSai.Code = "KH799999";
        TuChoi(() => bus.Sua(cu.Code, maSai), "Không thể đổi mã KH tại BUS");
        bus.Sua(cu.Code, DuLieuKhachHang.TuDuLieu(cu));
        KiemTra(dao.DocTheoMa(cu.Code) == cu, "Khách hàng giữ email của chính mình khi sửa");
    }

    private static void KiemTraEmailNhanVien(string chuoi)
    {
        var dao = new NhanVienDAO(chuoi);
        var bus = new NhanVienBUS(dao);
        var a = TaoNhanVien("NV700001", "Nhân viên A"); a.Email = "nv-a@example.test";
        var b = TaoNhanVien("NV700002", "Nhân viên B"); b.Email = "nv-b@example.test";
        bus.Them(a); var cu = bus.Them(b);
        dao.Sua(dao.DocTheoMa(a.Code)! with { Email = "nv-changed@example.test" });
        b.Email = "NV-CHANGED@example.test";
        TuChoi(() => bus.Sua(b.Code, b), "UNIQUE email NV khi UPDATE với cache cũ giữa hai phiên", 1062);
        KiemTra(dao.DocTheoMa(b.Code) == cu && bus.LayTheoMa(b.Code) == cu,
            "UPDATE NV bị UNIQUE từ chối không đổi dữ liệu/cache");
        var them = TaoNhanVien("NV700003", "Nhân viên C"); them.Email = b.Email;
        TuChoi(() => bus.Them(them), "UNIQUE email NV khi INSERT với cache cũ", 1062);
        KiemTra(dao.DocTheoMa(them.Code) is null && bus.TimKiem(them.Code).Count == 0,
            "INSERT NV bị UNIQUE từ chối không thêm cache");
        var maSai = DuLieuNhanVien.TuDuLieu(cu); maSai.Code = "NV799999";
        TuChoi(() => bus.Sua(cu.Code, maSai), "Không thể đổi mã NV tại BUS");
        bus.Sua(cu.Code, DuLieuNhanVien.TuDuLieu(cu));
        KiemTra(dao.DocTheoMa(cu.Code) == cu, "Nhân viên giữ email của chính mình khi sửa");
    }

    private static async Task KiemTraHaiPhien(string chuoi)
    {
        var dao = new KhachHangDAO(chuoi);
        var a = new KhachHangBUS(dao);
        var b = new KhachHangBUS(new KhachHangDAO(chuoi));
        _ = a.TimKiem(); _ = b.TimKiem();
        bool Them(KhachHangBUS bus)
        {
            try { bus.Them(TaoKhachHang("KH800001", "Tạo đồng thời")); return true; }
            catch (InvalidOperationException ex) when (ex.InnerException is MySqlException { Number: 1062 }) { return false; }
        }
        var ketQua = await Task.WhenAll(Task.Run(() => Them(a)), Task.Run(() => Them(b)));
        KiemTra(ketQua.Count(value => value) == 1, "Hai phiên thêm cùng mã: đúng một lần ghi thành công");
        KiemTra(dao.DocDanhSach().Count(value => value.Code == "KH800001") == 1, "MySQL không có hai KH cùng mã sau tranh chấp");
        KiemTra(a.TimKiem("KH800001").Count + b.TimKiem("KH800001").Count == 1,
            "Phiên thua tranh chấp không cập nhật cache sai");
        a.TaiLai(); b.TaiLai();
        KiemTra(a.LayTheoMa("KH800001") == b.LayTheoMa("KH800001"), "Tải lại giúp hai phiên thấy cùng dữ liệu đã lưu");

        var nv = new NhanVienDAO(chuoi);
        var nvA = new NhanVienBUS(nv); var nvB = new NhanVienBUS(new NhanVienDAO(chuoi));
        _ = nvA.TimKiem(); _ = nvB.TimKiem();
        nvA.Them(TaoNhanVien("NV800001", "Thêm trước"));
        TuChoi(() => nvB.Them(TaoNhanVien("NV800001", "Thêm sau")), "PK nhân viên từ phiên khác được chuyển thành thông báo", 1062);
        KiemTra(nvB.TimKiem("NV800001").Count == 0 && nv.DocTheoMa("NV800001")!.Name == "Thêm trước",
            "Trùng mã nhân viên giữ nguyên bản ghi/cache");
        var ncc = new NhaCungCapDAO(chuoi);
        var nccA = new NhaCungCapBUS(ncc); var nccB = new NhaCungCapBUS(new NhaCungCapDAO(chuoi));
        _ = nccA.TimKiem(); _ = nccB.TimKiem();
        nccA.Them(TaoNhaCungCap("N80001", "Thêm trước"));
        TuChoi(() => nccB.Them(TaoNhaCungCap("N80001", "Thêm sau")), "PK NCC từ phiên khác được chuyển thành thông báo", 1062);
        KiemTra(nccB.TimKiem("N80001").Count == 0 && ncc.DocTheoMa("N80001")!.Name == "Thêm trước",
            "Trùng mã NCC giữ nguyên bản ghi/cache");
        nccB.TaiLai();
        var cu = nccB.LayTheoMa("N80001");
        ncc.Xoa(cu.Code);
        TuChoi(() => nccB.Sua(cu.Code, TaoNhaCungCap(cu.Code, "Sửa hồ sơ đã bị xóa")),
            "Sửa NCC bị phiên khác xóa trả lỗi không tìm thấy");
        KiemTra(nccB.LayTheoMa(cu.Code) == cu && ncc.DocTheoMa(cu.Code) is null,
            "Sửa NCC thất bại không ghi cache giả hoặc tạo lại bản ghi");
        nccB.TaiLai();
        TuChoi(() => nccB.LayTheoMa(cu.Code), "Tải lại loại hồ sơ NCC đã bị phiên khác xóa");
    }

    private static DuLieuKhachHang TaoKhachHang(string ma, string ten) => new()
        { Code = ma, Name = ten, Phone = "0900000000", Address = "Địa chỉ kiểm thử" };
    private static DuLieuNhanVien TaoNhanVien(string ma, string ten) => new()
        { Code = ma, Name = ten, Position = "Nhân viên", Phone = "", Email = "" };
    private static DuLieuNhaCungCap TaoNhaCungCap(string ma, string ten) => new()
        { Code = ma, Name = ten, Phone = "", Email = "", Address = "" };

    private static void ThucThi(string chuoi, string sql)
    {
        using var ketNoi = new MyConnection(chuoi).MoKetNoi();
        using var lenh = new MySqlCommand(sql, ketNoi);
        lenh.ExecuteNonQuery();
    }
    private static void KiemTra(bool dung, string moTa)
    {
        if (!dung) throw new InvalidOperationException(moTa);
        soKiemTra++;
        Console.WriteLine("PASS MYSQL " + moTa);
    }
    private static void TuChoi(Action thaoTac, string moTa, int? maLoiMySql = null)
    {
        try { thaoTac(); }
        catch (InvalidOperationException ex) when (maLoiMySql is null || ex.InnerException is MySqlException sql && sql.Number == maLoiMySql)
        {
            KiemTra(true, moTa);
            return;
        }
        throw new InvalidOperationException("Chưa từ chối đúng lỗi: " + moTa);
    }
}
