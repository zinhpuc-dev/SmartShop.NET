using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO;

public sealed record KhachHangDTO(string Code, string Name, string Phone, string Email, string Address);

public sealed class DuLieuKhachHang
{
    [Required(ErrorMessage = "Vui lòng nhập mã khách hàng.")]
    [StringLength(8, ErrorMessage = "Mã khách hàng tối đa 8 ký tự.")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên cần từ 2 đến 150 ký tự.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string Phone { get; set; } = "";

    [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [StringLength(300, ErrorMessage = "Địa chỉ tối đa 300 ký tự.")]
    public string Address { get; set; } = "";

    public static DuLieuKhachHang TuDuLieu(KhachHangDTO value) => new()
    {
        Code = value.Code, Name = value.Name, Phone = value.Phone,
        Email = value.Email, Address = value.Address
    };
}
