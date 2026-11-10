using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO;

public sealed record Employee(string Code, string Name, string Position, string Phone, string Email);

/// <summary>Bản sao dùng để chỉnh sửa; hủy form không làm đổi nhân viên đã lưu.</summary>
public sealed class EmployeeDraft
{
    [Required(ErrorMessage = "Vui lòng nhập mã nhân viên.")]
    [StringLength(8, ErrorMessage = "Mã nhân viên tối đa 8 ký tự.")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên cần từ 2 đến 150 ký tự.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập chức vụ.")]
    [StringLength(100, ErrorMessage = "Chức vụ tối đa 100 ký tự.")]
    public string Position { get; set; } = "";

    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string Phone { get; set; } = "";

    [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự.")]
    public string Email { get; set; } = "";

    public static EmployeeDraft From(Employee value) => new()
    {
        Code = value.Code, Name = value.Name, Position = value.Position,
        Phone = value.Phone, Email = value.Email
    };
}
