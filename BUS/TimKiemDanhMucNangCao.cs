using System.Globalization;
using SmartShop.DTO;

namespace SmartShop.BUS;

// Dùng chung định nghĩa trường để giao diện và BUS kiểm tra cùng một bộ điều kiện.
public static class TimKiemDanhMucNangCao
{
    private static readonly CotTimKiem<KhachHangDTO>[] customerColumns =
    [
        VanBan<KhachHangDTO>("Code", "Mã khách hàng", value => value.Code),
        VanBan<KhachHangDTO>("Name", "Họ tên", value => value.Name),
        VanBan<KhachHangDTO>("Phone", "Điện thoại", value => value.Phone),
        VanBan<KhachHangDTO>("Email", "Email", value => value.Email),
        VanBan<KhachHangDTO>("Address", "Địa chỉ", value => value.Address)
    ];
    private static readonly CotTimKiem<NhanVienDTO>[] employeeColumns =
    [
        VanBan<NhanVienDTO>("Code", "Mã nhân viên", value => value.Code),
        VanBan<NhanVienDTO>("Name", "Họ tên", value => value.Name),
        VanBan<NhanVienDTO>("Position", "Chức vụ", value => value.Position),
        VanBan<NhanVienDTO>("Phone", "Điện thoại", value => value.Phone),
        VanBan<NhanVienDTO>("Email", "Email", value => value.Email)
    ];
    private static readonly CotTimKiem<NhaCungCapDTO>[] supplierColumns =
    [
        VanBan<NhaCungCapDTO>("Code", "Mã nhà cung cấp", value => value.Code),
        VanBan<NhaCungCapDTO>("Name", "Tên nhà cung cấp", value => value.Name),
        VanBan<NhaCungCapDTO>("Phone", "Điện thoại", value => value.Phone),
        VanBan<NhaCungCapDTO>("Email", "Email", value => value.Email),
        VanBan<NhaCungCapDTO>("Address", "Địa chỉ", value => value.Address)
    ];
    private static readonly CotTimKiem<SanPhamQuanTriDTO>[] productColumns =
    [
        VanBan<SanPhamQuanTriDTO>("Code", "Mã sản phẩm", value => value.MaSP),
        VanBan<SanPhamQuanTriDTO>("Name", "Tên sản phẩm", value => value.TenSP),
        VanBan<SanPhamQuanTriDTO>("Category", "Mã loại", value => value.MaLoai),
        VanBan<SanPhamQuanTriDTO>("Unit", "Đơn vị tính", value => value.DonViTinh),
        So<SanPhamQuanTriDTO>("Price", "Giá bán", value => value.GiaBan),
        So<SanPhamQuanTriDTO>("ListPrice", "Giá niêm yết", value => value.GiaNiemYet),
        So<SanPhamQuanTriDTO>("Stock", "Tồn kho", value => value.TongSoLuongTon),
        VanBan<SanPhamQuanTriDTO>("Status", "Trạng thái", value => value.TrangThai == "1" ? "Đang bán" : "Ngừng bán")
    ];

    public static IReadOnlyList<TruongTimKiem> TruongKhachHang { get; } = LayCacTruong(customerColumns);
    public static IReadOnlyList<TruongTimKiem> TruongNhanVien { get; } = LayCacTruong(employeeColumns);
    public static IReadOnlyList<TruongTimKiem> TruongNhaCungCap { get; } = LayCacTruong(supplierColumns);
    public static IReadOnlyList<TruongTimKiem> TruongSanPham { get; } = LayCacTruong(productColumns);

    internal static Func<KhachHangDTO, bool> LocKhachHang(BoLocDanhMuc? filter) => TaoBoLoc(filter, customerColumns);
    internal static Func<NhanVienDTO, bool> LocNhanVien(BoLocDanhMuc? filter) => TaoBoLoc(filter, employeeColumns);
    internal static Func<NhaCungCapDTO, bool> LocNhaCungCap(BoLocDanhMuc? filter) => TaoBoLoc(filter, supplierColumns);
    internal static Func<SanPhamQuanTriDTO, bool> LocSanPham(BoLocDanhMuc? filter) => TaoBoLoc(filter, productColumns);

    private static IReadOnlyList<TruongTimKiem> LayCacTruong<T>(CotTimKiem<T>[] columns) =>
        Array.AsReadOnly(columns.Select(column => column.Field).ToArray());

    private static CotTimKiem<T> VanBan<T>(string key, string label, Func<T, string> select) =>
        new(new TruongTimKiem(key, label), select, null);

    private static CotTimKiem<T> So<T>(string key, string label, Func<T, decimal> select) =>
        new(new TruongTimKiem(key, label, KieuTruongTimKiem.So), null, select);

    private static Func<T, bool> TaoBoLoc<T>(BoLocDanhMuc? filter, CotTimKiem<T>[] columns)
    {
        if (filter is null) return _ => true;
        if (filter.Groups is null || filter.Groups.Count > 8)
            throw new ArgumentException("Bộ lọc hỗ trợ tối đa 8 nhóm OR.");
        if (filter.Groups.Count == 0) return _ => true;

        var groups = new List<Func<T, bool>[]>();
        foreach (var group in filter.Groups)
        {
            if (group?.Conditions is null || group.Conditions.Count is 0 or > 8)
                throw new ArgumentException("Mỗi nhóm cần từ 1 đến 8 điều kiện AND.");
            groups.Add(group.Conditions.Select(condition => TaoDieuKien(condition, columns)).ToArray());
        }
        // Kiểm tra toàn bộ điều kiện trước khi lọc, kể cả khi danh mục đang rỗng.
        return value => groups.Any(group => group.All(matches => matches(value)));
    }

    private static Func<T, bool> TaoDieuKien<T>(DieuKienTimKiem condition, CotTimKiem<T>[] columns)
    {
        if (condition is null || !Enum.IsDefined(condition.Operator))
            throw new ArgumentException("Phép so sánh không hợp lệ.");
        var column = columns.SingleOrDefault(value => value.Field.Key == condition.Field)
            ?? throw new ArgumentException("Trường tìm kiếm không hợp lệ cho danh mục này.");
        if (condition.Value is null || condition.Value.Length > 500)
            throw new ArgumentException("Giá trị tìm kiếm tối đa 500 ký tự.");

        Func<T, bool> matches;
        if (column.So is not null)
        {
            if (condition.Operator == PhepSoSanh.Chua)
                throw new ArgumentException($"{column.Field.Label} cần phép so sánh số.");
            var input = condition.Value.Trim().Replace(',', '.');
            if (!decimal.TryParse(input, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var number))
                throw new ArgumentException($"{column.Field.Label} cần một số hợp lệ, không dùng dấu phân cách hàng nghìn.");
            matches = value => SoSanh(column.So(value).CompareTo(number), condition.Operator);
        }
        else
        {
            if (condition.Operator is not (PhepSoSanh.Chua or PhepSoSanh.Bang or PhepSoSanh.Khac))
                throw new ArgumentException($"{column.Field.Label} chỉ hỗ trợ Chứa, = và <>.");
            var term = XuLyChuoiTimKiem.ChuanHoa(condition.Value.Trim());
            matches = value => SoKhopChuoi(XuLyChuoiTimKiem.ChuanHoa(column.VanBan!(value) ?? ""), term, condition.Operator);
        }
        return condition.Negate ? value => !matches(value) : matches;
    }

    private static bool SoKhopChuoi(string value, string term, PhepSoSanh operation) => operation switch
    {
        PhepSoSanh.Chua => value.Contains(term, StringComparison.OrdinalIgnoreCase),
        PhepSoSanh.Bang => value.Equals(term, StringComparison.OrdinalIgnoreCase),
        PhepSoSanh.Khac => !value.Equals(term, StringComparison.OrdinalIgnoreCase),
        _ => throw new ArgumentException("Phép so sánh văn bản không hợp lệ.")
    };

    private static bool SoSanh(int result, PhepSoSanh operation) => operation switch
    {
        PhepSoSanh.Bang => result == 0,
        PhepSoSanh.Khac => result != 0,
        PhepSoSanh.LonHon => result > 0,
        PhepSoSanh.LonHonHoacBang => result >= 0,
        PhepSoSanh.NhoHon => result < 0,
        PhepSoSanh.NhoHonHoacBang => result <= 0,
        _ => throw new ArgumentException("Phép so sánh số không hợp lệ.")
    };

    private sealed record CotTimKiem<T>(TruongTimKiem Field, Func<T, string>? VanBan, Func<T, decimal>? So);
}
