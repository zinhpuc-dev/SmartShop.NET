using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SmartShop.BUS;
using SmartShop.DTO;

namespace SmartShop.UI.Shared;

// Các thao tác giao diện dùng chung; nghiệp vụ do lớp BUS xử lý.
public abstract class TrangCoSo : ComponentBase, IDisposable
{
    [Inject] protected ICuaHangBUS Shop { get; set; } = default!;
    [Inject] protected TrangThaiGiaoDien Ui { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected string page => new Uri(Navigation.Uri).AbsolutePath.Trim('/').ToLowerInvariant() switch
    {
        "" => "home",
        var path => path
    };
    protected string PageName => page switch
    {
        "home" => "Trang chủ", "cart" => "Giỏ hàng", "checkout" => "Thanh toán",
        "success" => "Đặt hàng thành công", "admin" => "Quản trị", "admin/customers" => "Khách hàng", "admin/employees" => "Nhân viên", "admin/suppliers" => "Nhà cung cấp", "admin/inventory" => "Kho & nhập hàng", "admin/promotions" => "Khuyến mại & voucher", "orders" => "Đơn hàng",
        "account" => "Tài khoản", "favorites" => "Yêu thích", "support" => "Hỗ trợ", _ => "Sản phẩm"
    };
    protected static readonly string[] categories = ["Tất cả", "Điện thoại", "Laptop", "Máy tính bảng", "Phụ kiện"];
    protected string query { get => Ui.Query; set { Ui.Query = value; Ui.ThongBaoThayDoi(); } }
    protected string category { get => Ui.Category; set { Ui.Category = value; Ui.ThongBaoThayDoi(); } }
    protected string sort { get => Ui.Sort; set { Ui.Sort = value; Ui.ThongBaoThayDoi(); } }
    protected string toast { get => Ui.Toast; set { Ui.Toast = value; Ui.ThongBaoThayDoi(); } }
    protected string adminTab { get => Ui.AdminTab; set { Ui.AdminTab = value; Ui.ThongBaoThayDoi(); } }
    protected HashSet<int> favorites => Ui.Favorites;
    protected SanPhamDTO? selected { get => Ui.Selected; set { Ui.Selected = value; Ui.ThongBaoThayDoi(); } }
    protected SanPhamDTO? editing { get => Ui.Editing; set { Ui.Editing = value; Ui.ThongBaoThayDoi(); } }
    protected DonHangDTO? lastOrder => Ui.LastOrder;
    protected ThongTinNhanHang checkout => Ui.ThanhToan;

    protected IEnumerable<SanPhamDTO> Filtered => Shop.TimKiem(query, category, page == "favorites" ? favorites : null, sort);

    protected override void OnInitialized()
    {
        Ui.Changed += LamMoiGiaoDien;
        Navigation.LocationChanged += DuongDanThayDoi;
    }
    private void LamMoiGiaoDien() => _ = InvokeAsync(StateHasChanged);
    private void DuongDanThayDoi(object? sender, LocationChangedEventArgs args) => LamMoiGiaoDien();
    public void Dispose()
    {
        Ui.Changed -= LamMoiGiaoDien;
        Navigation.LocationChanged -= DuongDanThayDoi;
        GC.SuppressFinalize(this);
    }
    protected static string BieuTuongDanhMuc(string c) => c switch
    {
        "Điện thoại" => "▯", "Laptop" => "▱", "Máy tính bảng" => "▣", _ => "♧"
    };
    protected void DieuHuong(string destination)
    {
        Ui.Query = "";
        Ui.Category = "Tất cả";
        Ui.Toast = "";
        Ui.Selected = null;
        Ui.Editing = null;
        Navigation.NavigateTo(destination == "home" ? "/" : $"/{destination}");
        Ui.ThongBaoThayDoi();
    }
    protected void LocDanhMuc(string c) { DieuHuong("products"); category = c; }
    protected void MoChiTiet(SanPhamDTO p) => selected = p;
    protected bool ThemVaoGio(SanPhamDTO p)
    {
        var ok = Shop.ThemVaoGio(Shop.Products.Single(x => x.Id == p.Id));
        toast = ok ? $"Đã thêm {p.Name} vào giỏ" : "Đã đạt số lượng tồn kho";
        return ok;
    }
    protected void DoiSoLuong(int id, int delta) { Shop.DoiSoLuong(id, delta); Ui.ThongBaoThayDoi(); }
    protected void XoaKhoiGio(int id) { Shop.XoaKhoiGio(id); Ui.ThongBaoThayDoi(); }
    protected void DoiYeuThich(int id)
    {
        if (!favorites.Add(id)) favorites.Remove(id);
        Ui.ThongBaoThayDoi();
    }
    protected void DatHang()
    {
        try
        {
            Ui.LastOrder = Shop.ThanhToan(checkout.Name.Trim(), checkout.Phone.Trim(), checkout.Address.Trim());
            DieuHuong("success");
        }
        catch (InvalidOperationException ex) { toast = ex.Message; }
    }
    protected void CapNhatTrangThaiDonHang(string code, string status)
    {
        try { Shop.CapNhatTrangThaiDonHang(code, status); Ui.ThongBaoThayDoi(); }
        catch (InvalidOperationException ex) { toast = ex.Message; }
    }
    protected void MoFormSuaSanPham(SanPhamDTO p)
    {
        editing = p;
    }
    protected void LuuSanPham()
    {
        if (editing is null) return;
        toast = "Chỉnh sửa sản phẩm chỉ khả dụng trong phiên demo.";
        editing = null;
    }
}
