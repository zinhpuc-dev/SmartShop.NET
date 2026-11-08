using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO
{
    public class ProductManage
    {
        // Thuộc tính 
        [Required(ErrorMessage = "Vui lòng nhập mã sản phẩm.")]
        [StringLength(8, ErrorMessage = "Mã sản phẩm tối đa 8 ký tự.")]
        public string MaSP { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm.")]
        [StringLength(6, ErrorMessage = "Mã loại tối đa 6 ký tự.")]
        public string MaLoai { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự.")]
        public string TenSP { get; set; } = string.Empty;
        [StringLength(65535, ErrorMessage = "Mô tả quá dài.")]
        public string MoTa { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính.")]
        [StringLength(30, ErrorMessage = "Đơn vị tính tối đa 30 ký tự.")]
        public string DonViTinh { get; set; } = string.Empty;
        [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "Giá niêm yết không hợp lệ.")]
        public decimal GiaNiemYet { get; set; }
        [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "Giá bán không hợp lệ.")]
        public decimal GiaBan { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int TongSoLuongTon { get; set; }

        // Constructor không tham số
        public ProductManage()
        {
        }

        // Constructor có tham số
        public ProductManage(string maSP, string maLoai, string tenSP, string moTa, string donViTinh, decimal giaNiemYet, decimal giaBan, string trangThai)
        {
            MaSP = maSP;
            MaLoai = maLoai;
            TenSP = tenSP;
            MoTa = moTa;
            DonViTinh = donViTinh;
            GiaNiemYet = giaNiemYet;
            GiaBan = giaBan;
            TrangThai = trangThai;
        }
    }
}