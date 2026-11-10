using SmartShop.DTO;
using System.ComponentModel.DataAnnotations;

namespace SmartShop.UI.Shared;

// Trạng thái giao diện theo phiên Blazor, dùng chung giữa bố cục và các trang.
public sealed class ShopUiState
{
    public event Action? Changed;
    public void Notify() => Changed?.Invoke();
    public string Query { get; set; } = "";
    public string Category { get; set; } = "Tất cả";
    public string Sort { get; set; } = "popular";
    public string Toast { get; set; } = "";
    public string AdminTab { get; set; } = "Sản phẩm";
    public HashSet<int> Favorites { get; } = [];
    public Product? Selected { get; set; }
    public Product? Editing { get; set; }
    public Order? LastOrder { get; set; }
    public CheckoutInput Checkout { get; } = new();
}

public sealed class CheckoutInput
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Họ tên không được để trống.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"0[0-9]{9}", ErrorMessage = "Số điện thoại gồm 10 số, bắt đầu bằng 0.")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [MinLength(10, ErrorMessage = "Địa chỉ cần ít nhất 10 ký tự.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Địa chỉ không được để trống.")]
    public string Address { get; set; } = "";
}
