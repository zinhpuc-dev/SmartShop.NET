using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO;

public sealed record Supplier(string Code, string Name, string Phone, string Email, string Address);

/// <summary>Bản sao dùng để chỉnh sửa; hủy form không làm đổi nhà cung cấp đã lưu.</summary>
public sealed class SupplierDraft
{
    [Required(ErrorMessage = "Vui lòng nhập mã nhà cung cấp.")]
    [StringLength(6, ErrorMessage = "Mã nhà cung cấp tối đa 6 ký tự.")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên nhà cung cấp cần từ 2 đến 200 ký tự.")]
    public string Name { get; set; } = "";

    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string Phone { get; set; } = "";

    [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự.")]
    public string Email { get; set; } = "";

    [StringLength(300, ErrorMessage = "Địa chỉ tối đa 300 ký tự.")]
    public string Address { get; set; } = "";

    public static SupplierDraft From(Supplier value) => new()
    {
        Code = value.Code, Name = value.Name, Phone = value.Phone,
        Email = value.Email, Address = value.Address
    };
}
