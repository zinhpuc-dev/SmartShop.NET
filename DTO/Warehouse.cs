using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO;

public sealed class Warehouse
{
    [Required(ErrorMessage = "Vui lòng nhập mã kho.")]
    [StringLength(6, ErrorMessage = "Mã kho tối đa 6 ký tự.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên kho.")]
    [StringLength(150, ErrorMessage = "Tên kho tối đa 150 ký tự.")]
    public string Name { get; set; } = string.Empty;
}
