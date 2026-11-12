using SmartShop.DTO;

namespace SmartShop.BUS;

/// <summary>Application operations exposed to User and Admin interfaces.</summary>
public interface ICuaHangBUS
{
    IReadOnlyList<SanPhamDTO> Products { get; }
    IReadOnlyList<DonHangDTO> Orders { get; }
    IReadOnlyDictionary<int, int> Cart { get; }
    int Count { get; }
    decimal Total { get; }
    IEnumerable<SanPhamDTO> TimKiem(string query, string category, HashSet<int>? favorites, string sort);
    bool ThemVaoGio(SanPhamDTO product);
    void DoiSoLuong(int id, int delta);
    void XoaKhoiGio(int id);
    void CapNhatSanPham(int id, decimal price, int stock);
    void CapNhatTrangThaiDonHang(string code, string status);
    DonHangDTO ThanhToan(string name, string phone, string address);
}
