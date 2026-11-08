using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO;

public sealed class ProductCategory
{
    [Required(ErrorMessage = "Vui lòng nhập mã loại sản phẩm.")]
    [StringLength(6, ErrorMessage = "Mã loại tối đa 6 ký tự.")]
    public string MaLoai { get; set; } = string.Empty;
    public bool TrangThai { get; set; } = true;
}
