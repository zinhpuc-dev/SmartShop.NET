using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SmartShop.BUS;
using SmartShop.DTO;

namespace SmartShop.UI.Shared;

// Các thao tác giao diện dùng chung; nghiệp vụ do lớp BUS xử lý.
public abstract class ShopPageBase : ComponentBase, IDisposable
{
    [Inject] protected IShopService Shop { get; set; } = default!;
    [Inject] protected ShopUiState Ui { get; set; } = default!;
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
    protected string query { get => Ui.Query; set { Ui.Query = value; Ui.Notify(); } }
    protected string category { get => Ui.Category; set { Ui.Category = value; Ui.Notify(); } }
    protected string sort { get => Ui.Sort; set { Ui.Sort = value; Ui.Notify(); } }
    protected string toast { get => Ui.Toast; set { Ui.Toast = value; Ui.Notify(); } }
    protected string adminTab { get => Ui.AdminTab; set { Ui.AdminTab = value; Ui.Notify(); } }
    protected HashSet<int> favorites => Ui.Favorites;
    protected Product? selected { get => Ui.Selected; set { Ui.Selected = value; Ui.Notify(); } }
    protected Product? editing { get => Ui.Editing; set { Ui.Editing = value; Ui.Notify(); } }
    protected Order? lastOrder => Ui.LastOrder;
    protected CheckoutInput checkout => Ui.Checkout;

    protected IEnumerable<Product> Filtered => Shop.Search(query, category, page == "favorites" ? favorites : null, sort);

    protected override void OnInitialized()
    {
        Ui.Changed += Refresh;
        Navigation.LocationChanged += LocationChanged;
    }
    private void Refresh() => _ = InvokeAsync(StateHasChanged);
    private void LocationChanged(object? sender, LocationChangedEventArgs args) => Refresh();
    public void Dispose()
    {
        Ui.Changed -= Refresh;
        Navigation.LocationChanged -= LocationChanged;
        GC.SuppressFinalize(this);
    }
    protected static string CategoryIcon(string c) => c switch
    {
        "Điện thoại" => "▯", "Laptop" => "▱", "Máy tính bảng" => "▣", _ => "♧"
    };
    protected void Go(string destination)
    {
        Ui.Query = "";
        Ui.Category = "Tất cả";
        Ui.Toast = "";
        Ui.Selected = null;
        Ui.Editing = null;
        Navigation.NavigateTo(destination == "home" ? "/" : $"/{destination}");
        Ui.Notify();
    }
    protected void Filter(string c) { Go("products"); category = c; }
    protected void Open(Product p) => selected = p;
    protected bool Add(Product p)
    {
        var ok = Shop.Add(Shop.Products.Single(x => x.Id == p.Id));
        toast = ok ? $"Đã thêm {p.Name} vào giỏ" : "Đã đạt số lượng tồn kho";
        return ok;
    }
    protected void Change(int id, int delta) { Shop.Change(id, delta); Ui.Notify(); }
    protected void Remove(int id) { Shop.Remove(id); Ui.Notify(); }
    protected void ToggleFavorite(int id)
    {
        if (!favorites.Add(id)) favorites.Remove(id);
        Ui.Notify();
    }
    protected void PlaceOrder()
    {
        try
        {
            Ui.LastOrder = Shop.Checkout(checkout.Name.Trim(), checkout.Phone.Trim(), checkout.Address.Trim());
            Go("success");
        }
        catch (InvalidOperationException ex) { toast = ex.Message; }
    }
    protected void UpdateOrderStatus(string code, string status)
    {
        try { Shop.UpdateOrderStatus(code, status); Ui.Notify(); }
        catch (InvalidOperationException ex) { toast = ex.Message; }
    }
    protected void EditProduct(Product p)
    {
        editing = p;
    }
    protected void SaveProduct()
    {
        if (editing is null) return;
        toast = "Chỉnh sửa sản phẩm chỉ khả dụng trong phiên demo.";
        editing = null;
    }
}
