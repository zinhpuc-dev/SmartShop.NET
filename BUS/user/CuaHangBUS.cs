using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

// The GUI calls this service; only BUS talks to DAO.
public sealed class CuaHangBUS : ICuaHangBUS
{
    private readonly ICuaHangDAO dao;
    private readonly Dictionary<int, int> cart = [];
    private readonly IReadOnlyDictionary<int, int> cartView;
    private string customerPhone = "";

    public CuaHangBUS(ICuaHangDAO repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        dao = repository;
        cartView = new ReadOnlyDictionary<int, int>(cart);
    }

    public IReadOnlyList<SanPhamDTO> Products => dao.DocSanPham();
    public IReadOnlyList<DonHangDTO> Orders => string.IsNullOrWhiteSpace(customerPhone) ? [] : dao.DocDonHang(customerPhone);
    public IReadOnlyDictionary<int, int> Cart => cartView;
    public int Count => cart.Values.Sum();
    public decimal Total => cart.Sum(x => TimSanPham(x.Key).Price * x.Value);

    public IEnumerable<SanPhamDTO> TimKiem(string query, string category, HashSet<int>? favorites, string sort)
    {
        // Filtering stays in BUS and works on an ArrayList loaded from DAO.
        var products = dao.NapMangSanPham().Cast<SanPhamDTO>().Where(p =>
            (category == "Tất cả" || p.Category == category)
            && (p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                || p.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
            && (favorites is null || favorites.Contains(p.Id)));
        return sort switch
        {
            "low" => products.OrderBy(p => p.Price),
            "high" => products.OrderByDescending(p => p.Price),
            _ => products
        };
    }

    public bool ThemVaoGio(SanPhamDTO product)
    {
        var current = TimSanPham(product.Id);
        if (cart.GetValueOrDefault(current.Id) >= current.Stock) return false;
        cart[current.Id] = cart.GetValueOrDefault(current.Id) + 1;
        return true;
    }
    public void DoiSoLuong(int id, int delta)
    {
        var product = TimSanPham(id);
        var quantity = cart.GetValueOrDefault(id) + delta;
        if (quantity <= 0) cart.Remove(id);
        else if (quantity <= product.Stock) cart[id] = quantity;
    }
    public void XoaKhoiGio(int id) => cart.Remove(id);

    public void CapNhatSanPham(int id, decimal price, int stock)
    {
        if (price <= 0 || stock < 0 || stock < cart.GetValueOrDefault(id))
            throw new InvalidOperationException("Kiểm tra giá và tồn kho (không thấp hơn số lượng trong giỏ).");
        dao.CapNhatSanPham(TimSanPham(id) with { Price = price, Stock = stock });
    }
    public void CapNhatTrangThaiDonHang(string code, string status)
    {
        if (status is not ("Chờ xác nhận" or "Đã xác nhận" or "Đang giao hàng" or "Hoàn thành"))
            throw new InvalidOperationException("Trạng thái đơn hàng không hợp lệ.");
        dao.CapNhatTrangThaiDonHang(code, status);
    }
    public DonHangDTO ThanhToan(string name, string phone, string address)
    {
        if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(phone, @"^0[0-9]{9}$")
            || string.IsNullOrWhiteSpace(address) || address.Trim().Length < 10)
            throw new InvalidOperationException("Vui lòng kiểm tra thông tin nhận hàng.");
        if (cart.Count == 0) throw new InvalidOperationException("Giỏ hàng đang trống.");
        var lines = cart.Select(x => new ChiTietDonHangDTO(TimSanPham(x.Key), x.Value)).ToList().AsReadOnly();
        if (lines.Any(x => x.Quantity <= 0 || x.Quantity > x.Product.Stock))
            throw new InvalidOperationException("Không đủ tồn kho.");
        var order = new DonHangDTO($"DH{Guid.NewGuid():N}"[..8].ToUpperInvariant(),
            name.Trim(), phone, address.Trim(), DateTime.Now, lines,
            lines.Sum(x => x.Product.Price * x.Quantity));
        order = dao.LuuDonHang(order);
        customerPhone = phone.Trim();
        cart.Clear();
        return order;
    }
    private SanPhamDTO TimSanPham(int id) => Products.SingleOrDefault(p => p.Id == id)
        ?? throw new InvalidOperationException("Không tìm thấy sản phẩm.");
}
