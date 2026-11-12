using SmartShop.BUS;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class KiemTraTimKiemDanhMuc
{
    private static void KiemTra(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS SEARCH " + message);
    }

    public static void Chay()
    {
        var customerData = new KhoKhachHangGiaLap();
        var customers = new KhachHangBUS(customerData);
        KiemTra(customers.TimKiem("  nguyen an ").Single().Code == "KH000001", "Customer accentless search");
        customers.TimKiem("090"); customers.TimKiem(""); customers.GoiYMa();
        KiemTra(customerData.Reads == 1, "Customer search loads once");
        var added = customers.Them(new DuLieuKhachHang
        {
            Code = "KH000002", Name = "Trần Bình", Phone = "0900000002", Address = "123 Đường Demo"
        });
        KiemTra(customers.TimKiem("tran binh").Single() == added, "Customer create updates cache");
        var draft = DuLieuKhachHang.TuDuLieu(added);
        draft.Name = "Lê Chi";
        customers.Sua(added.Code, draft);
        KiemTra(customers.TimKiem("le chi").Count == 1 && customers.TimKiem("tran binh").Count == 0,
            "Customer edit updates cache");
        customerData.FailWrites = true;
        try { customers.Xoa(added.Code); throw new Exception("Delete should fail"); }
        catch (InvalidOperationException) { }
        KiemTra(customers.TimKiem("le chi").Count == 1, "Failed write preserves cache");
        customerData.FailWrites = false;
        customers.Xoa(added.Code);
        KiemTra(customers.TimKiem("le chi").Count == 0, "Customer delete updates cache");
        customers.TaiLai(); customers.TimKiem(""); customers.TimKiem("");
        KiemTra(customerData.Reads == 2, "Explicit reload reads once");

        var employeeData = new KhoNhanVienGiaLap();
        var employees = new NhanVienBUS(employeeData);
        KiemTra(employees.TimKiem("quan ly").Single().Code == "NV000001", "Employee searches position without accents");
        employees.TimKiem("minh"); employees.TimKiem("");
        KiemTra(employeeData.Reads == 1, "Employee search loads once");
        var employeeDraft = DuLieuNhanVien.TuDuLieu(employeeData.Row);
        employeeDraft.Name = "Đỗ Hà";
        employees.Sua(employeeDraft.Code, employeeDraft);
        KiemTra(employees.TimKiem("do ha").Count == 1, "Employee edit updates cache");
        employees.Xoa(employeeDraft.Code);
        KiemTra(employees.TimKiem("").Count == 0, "Employee delete updates cache");

        var supplierData = new KhoNhaCungCapGiaLap();
        var suppliers = new NhaCungCapBUS(supplierData);
        KiemTra(suppliers.TimKiem("anh duong").Single().Code == "N00001", "Supplier accentless search");
        suppliers.TimKiem(""); suppliers.TimKiem("example");
        KiemTra(supplierData.Reads == 1, "Supplier search loads once");
        supplierData.Row = supplierData.Row with { Name = "Tên đổi bởi phiên khác" };
        var supplierDraft = DuLieuNhaCungCap.TuDuLieu(supplierData.Row);
        supplierDraft.Name = "Đông Nam";
        suppliers.Sua(supplierDraft.Code, supplierDraft);
        KiemTra(suppliers.TimKiem("dong nam").Count == 1, "Supplier replaces cache by code after external edit");
        suppliers.Xoa(supplierDraft.Code);
        KiemTra(suppliers.TimKiem("").Count == 0, "Supplier delete updates cache");

        var productData = new KhoSanPhamGiaLap();
        var products = new SanPhamBUS(productData);
        KiemTra(products.TimKiem("dien thoai").Count == 1, "Product accentless search");
        KiemTra(products.TimKiem("SP000001", "L00001").Count == 1
            && products.TimKiem("SP000001", "L00002").Count == 0, "Product code and category filter");
        products.TimKiem(""); products.TimKiem("missing");
        KiemTra(productData.Reads == 1 && productData.Searches == 0, "Product search stays in BUS");
        var result = products.TimKiem("").Single();
        result.TenSP = "Changed result";
        KiemTra(products.TimKiem("dien thoai").Count == 1, "Mutable result cannot alter product cache");
        products.DoiTrangThai("SP000001", false);
        KiemTra(products.TimKiem("").Single().TrangThai == "0", "Product status refreshes cache");
        productData.Row = productData.Row with { TenSP = "Máy tính" };
        products.DocDanhSach();
        KiemTra(products.TimKiem("may tinh").Count == 1, "Product explicit read refreshes cache");
    }

    private static BoLocDanhMuc TaoBoLoc(params DieuKienTimKiem[] conditions) => new([new NhomDieuKien(conditions)]);

    private static void KiemTraTuChoiBoLoc(Action action, string message)
    {
        try { action(); }
        catch (ArgumentException) { KiemTra(true, message); return; }
        throw new Exception(message);
    }

    public static void ChayTimKiemNangCao()
    {
        var data = new KhoSanPhamGiaLap
        {
            Rows =
            [
                new("SP000001", "L00001", "Điện thoại Ánh Dương", "", "Chiếc", 120, 100.5m, "1") { TongSoLuongTon = 5 },
                new("SP000002", "L00001", "Điện thoại Sao Việt", "", "Chiếc", 60, 50, "0") { TongSoLuongTon = 0 },
                new("SP000003", "L00002", "Máy tính Đông Nam", "", "Chiếc", 220, 200, "1") { TongSoLuongTon = 20 }
            ]
        };
        var products = new SanPhamBUS(data);
        var comparisons = new[]
        {
            (PhepSoSanh.Bang, 1), (PhepSoSanh.Khac, 2), (PhepSoSanh.LonHon, 1),
            (PhepSoSanh.LonHonHoacBang, 2), (PhepSoSanh.NhoHon, 1), (PhepSoSanh.NhoHonHoacBang, 2)
        };
        foreach (var (operation, count) in comparisons)
            KiemTra(products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Price", operation, "100.5"))).Count == count,
                "Numeric operator " + operation);
        KiemTra(products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Price", PhepSoSanh.Bang, "100,5"))).Count == 1,
            "Vietnamese decimal separator");
        var combined = new BoLocDanhMuc([
            new NhomDieuKien([
                new("Price", PhepSoSanh.LonHon, "50"),
                new("Stock", PhepSoSanh.NhoHonHoacBang, "10"),
                new("Stock", PhepSoSanh.Bang, "0", true)]),
            new NhomDieuKien([new("Name", PhepSoSanh.Chua, "dong nam")])]);
        KiemTra(products.TimKiem("", null, combined).Select(value => value.MaSP).SequenceEqual(["SP000001", "SP000003"]),
            "AND groups joined with OR and NOT");
        KiemTra(products.TimKiem("dien thoai", "L00001", combined).Single().MaSP == "SP000001",
            "Advanced filter combines with existing query and category");
        KiemTra(products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Name", PhepSoSanh.Chua, "dien thoai", true))).Single().MaSP == "SP000003",
            "Negated text condition");
        KiemTra(products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Status", PhepSoSanh.Bang, "ngung ban"))).Single().MaSP == "SP000002",
            "Status uses accentless labels");
        KiemTra(products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Category", PhepSoSanh.Khac, "L00001"))).Count == 1,
            "Text inequality");
        KiemTra(products.TimKiem("", null, new BoLocDanhMuc([])).Count == 3, "Empty filter restores all rows");
        KiemTraTuChoiBoLoc(() => products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Price", PhepSoSanh.Bang, "abc"))), "Reject invalid numeric input");
        KiemTraTuChoiBoLoc(() => products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Price", PhepSoSanh.Bang, "1,000.5"))), "Reject grouped or ambiguous number");
        KiemTraTuChoiBoLoc(() => products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Missing", PhepSoSanh.Bang, "x"))), "Reject unknown field");
        KiemTraTuChoiBoLoc(() => products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Name", PhepSoSanh.LonHon, "x"))), "Reject numeric comparison on text");
        KiemTraTuChoiBoLoc(() => products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Price", PhepSoSanh.Chua, "1"))), "Reject contains on numeric field");
        KiemTraTuChoiBoLoc(() => products.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Price", (PhepSoSanh)999, "1"))), "Reject invalid operator");
        KiemTraTuChoiBoLoc(() => products.TimKiem("", null, new BoLocDanhMuc([new NhomDieuKien([])])), "Reject empty AND group");
        KiemTra(data.Reads == 1 && data.Searches == 0, "Advanced search never rereads DAO while filtering");

        var empty = new SanPhamBUS(new KhoSanPhamGiaLap { Rows = [] });
        KiemTraTuChoiBoLoc(() => empty.TimKiem("", null, TaoBoLoc(new DieuKienTimKiem("Price", PhepSoSanh.Bang, "bad"))), "Validate filter even for empty catalog");
        KiemTra(empty.TimKiem("", null, combined).Count == 0, "Valid expression handles empty catalog");

        var customers = new KhachHangBUS(new KhoKhachHangGiaLap());
        KiemTra(customers.TimKiem("", TaoBoLoc(new DieuKienTimKiem("Name", PhepSoSanh.Bang, "nguyen an"),
            new("Email", PhepSoSanh.Bang, ""))).Count == 1, "Customer advanced search and empty email");
        var employees = new NhanVienBUS(new KhoNhanVienGiaLap());
        KiemTra(employees.TimKiem("", TaoBoLoc(new DieuKienTimKiem("Position", PhepSoSanh.Chua, "quan ly"))).Count == 1,
            "Employee advanced search by position");
        var suppliers = new NhaCungCapBUS(new KhoNhaCungCapGiaLap());
        KiemTra(suppliers.TimKiem("", TaoBoLoc(new DieuKienTimKiem("Name", PhepSoSanh.Chua, "anh duong"),
            new("Email", PhepSoSanh.Chua, "example"))).Count == 1, "Supplier multi-field advanced search");
        KiemTraTuChoiBoLoc(() => suppliers.TimKiem("", TaoBoLoc(new DieuKienTimKiem("Stock", PhepSoSanh.Bang, "0"))), "Catalog field boundaries enforced in BUS");
    }

    public static void ChayKiemTraRangBuoc()
    {
        var repository = new KhoKhachHangGiaLap();
        var customers = new KhachHangBUS(repository);
        var first = DuLieuKhachHang.TuDuLieu(customers.LayTheoMa("KH000001"));
        first.Email = "an@example.com";
        customers.Sua(first.Code, first);
        var second = new DuLieuKhachHang
        {
            Code = "KH000002", Name = "Trần Bình", Phone = "0900000002", Address = "123 Đường Demo",
            Email = "AN@example.com"
        };
        KiemTraTuChoiGhi(() => customers.Them(second), "Customer duplicate email rejected by BUS");
        second.Email = "";
        customers.Them(second);
        var edit = DuLieuKhachHang.TuDuLieu(customers.LayTheoMa(second.Code));
        edit.Email = "an@example.com";
        KiemTraTuChoiGhi(() => customers.Sua(second.Code, edit), "Customer edit rejects duplicate email");
        KiemTra(customers.LayTheoMa(second.Code).Email == "", "Duplicate email preserves stored customer");
        edit.Code = "KH000009";
        KiemTraTuChoiGhi(() => customers.Sua(second.Code, edit), "Readonly customer code enforced by BUS");
        customers.Sua(first.Code, first);
        KiemTra(customers.LayTheoMa(first.Code).Email == first.Email, "Customer can keep its own email");
        repository.FailWrites = true;
        first.Name = "Không lưu được";
        KiemTraTuChoiGhi(() => customers.Sua(first.Code, first), "Storage failure does not commit customer edit");
        KiemTra(customers.TimKiem("Không lưu được").Count == 0, "Failed customer edit preserves search cache");

        var employeeData = new KhoNhanVienGiaLap();
        employeeData.Row = employeeData.Row with { Email = "minh@example.com" };
        var employees = new NhanVienBUS(employeeData);
        var duplicate = new DuLieuNhanVien
        {
            Code = "NV000002", Name = "Trần Hà", Position = "Bán hàng", Email = " MINH@example.com "
        };
        KiemTraTuChoiGhi(() => employees.Them(duplicate), "Employee duplicate email rejected before DAO insert");
        var employeeEdit = DuLieuNhanVien.TuDuLieu(employeeData.Row);
        employees.Sua(employeeEdit.Code, employeeEdit);
        KiemTra(employees.LayTheoMa(employeeEdit.Code).Email == employeeEdit.Email, "Employee can keep its own email");
        employeeEdit.Code = "NV000003";
        KiemTraTuChoiGhi(() => employees.Sua("NV000001", employeeEdit), "Readonly employee code enforced by BUS");

        var suppliers = new NhaCungCapBUS(new KhoNhaCungCapGiaLap());
        var supplierEdit = DuLieuNhaCungCap.TuDuLieu(suppliers.LayTheoMa("N00001"));
        supplierEdit.Code = "N00002";
        KiemTraTuChoiGhi(() => suppliers.Sua("N00001", supplierEdit), "Readonly supplier code enforced by BUS");
    }

    private static void KiemTraTuChoiGhi(Action action, string message)
    {
        try { action(); }
        catch (InvalidOperationException) { KiemTra(true, message); return; }
        throw new Exception(message);
    }

    private sealed class KhoKhachHangGiaLap : IKhachHangDAO
    {
        private readonly List<KhachHangDTO> rows = [new("KH000001", "Nguyễn An", "0900000001", "", "123 Đường Demo")];
        public int Reads { get; private set; }
        public bool FailWrites { get; set; }
        public IReadOnlyList<KhachHangDTO> DocDanhSach() { Reads++; return rows.ToArray(); }
        public KhachHangDTO? DocTheoMa(string code) => rows.SingleOrDefault(row => row.Code == code);
        public KhachHangDTO Them(KhachHangDTO value) { KiemTraQuyenGhi(); rows.Add(value); return value; }
        public KhachHangDTO Sua(KhachHangDTO value)
        {
            KiemTraQuyenGhi();
            rows[rows.FindIndex(row => row.Code == value.Code)] = value;
            return value;
        }
        public void Xoa(string code) { KiemTraQuyenGhi(); rows.RemoveAll(row => row.Code == code); }
        private void KiemTraQuyenGhi()
        {
            if (FailWrites) throw new InvalidOperationException("Simulated write failure");
        }
    }

    private sealed class KhoNhanVienGiaLap : INhanVienDAO
    {
        public NhanVienDTO Row = new("NV000001", "Nguyễn Minh", "Quản lý", "", "");
        private bool deleted;
        public int Reads { get; private set; }
        public IReadOnlyList<NhanVienDTO> DocDanhSach() { Reads++; return deleted ? [] : [Row]; }
        public NhanVienDTO? DocTheoMa(string code) => deleted ? null : Row;
        public NhanVienDTO Them(NhanVienDTO value) => throw new NotSupportedException();
        public NhanVienDTO Sua(NhanVienDTO value) => Row = value;
        public void Xoa(string code) => deleted = true;
    }

    private sealed class KhoNhaCungCapGiaLap : INhaCungCapDAO
    {
        public NhaCungCapDTO Row = new("N00001", "Ánh Dương", "", "demo@example.com", "");
        private bool deleted;
        public int Reads { get; private set; }
        public IReadOnlyList<NhaCungCapDTO> DocDanhSach() { Reads++; return deleted ? [] : [Row]; }
        public NhaCungCapDTO? DocTheoMa(string code) => deleted ? null : Row;
        public NhaCungCapDTO Them(NhaCungCapDTO value) => throw new NotSupportedException();
        public NhaCungCapDTO Sua(NhaCungCapDTO value) => Row = value;
        public void Xoa(string code) => deleted = true;
    }

    private sealed class KhoSanPhamGiaLap : ISanPhamDAO
    {
        public SanPhamQuanTriDTO Row = new("SP000001", "L00001", "Điện thoại", "", "Chiếc", 100, 90, "1");
        public int Reads { get; private set; }
        public int Searches { get; private set; }
        public List<SanPhamQuanTriDTO>? Rows { get; init; }
        public List<SanPhamQuanTriDTO> DocDanhSach() { Reads++; return (Rows ?? [Row]).Select(value => value with { }).ToList(); }
        public List<SanPhamQuanTriDTO> TimKiem(string keyword) { Searches++; throw new Exception("DAO search must not run"); }
        public SanPhamQuanTriDTO? DocTheoMa(string code) => Row with { };
        public bool KiemTraSanPham(string code) => code == Row.MaSP;
        public void DoiTrangThai(string code, bool active) => Row.TrangThai = active ? "1" : "0";
        public List<LoaiSanPhamDTO> DocLoaiSanPham() => [];
        public bool KiemTraLoaiSanPham(string code) => true;
        public void ThemLoaiSanPham(LoaiSanPhamDTO value) => throw new NotSupportedException();
        public void SuaLoaiSanPham(LoaiSanPhamDTO value) => throw new NotSupportedException();
        public void XoaLoaiSanPham(string code) => throw new NotSupportedException();
        public List<TonKhoSanPhamDTO> DocTonKhoTheoSanPham(string code) => [];
        public void Them(SanPhamQuanTriDTO value) => throw new NotSupportedException();
        public void Sua(SanPhamQuanTriDTO value) => Row = value with { };
        public void Xoa(string code) => throw new NotSupportedException();
    }
}
