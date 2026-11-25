using SmartShop.DTO;

namespace SmartShop.DTO.Contracts;

public interface ISanPhamDAO
{
    List<SanPhamQuanTriDTO> DocDanhSach();
    List<SanPhamQuanTriDTO> TimKiem(string keyword);
    SanPhamQuanTriDTO? DocTheoMa(string maSP);
    List<LoaiSanPhamDTO> DocLoaiSanPham();
    bool KiemTraLoaiSanPham(string maLoai);
    void ThemLoaiSanPham(LoaiSanPhamDTO category);
    void SuaLoaiSanPham(LoaiSanPhamDTO category);
    void XoaLoaiSanPham(string maLoai);
    List<TonKhoSanPhamDTO> DocTonKhoTheoSanPham(string maSP);
    bool KiemTraSanPham(string maSP);
    void Them(SanPhamQuanTriDTO product);
    void Sua(SanPhamQuanTriDTO product);
    void Xoa(string maSP);
    void DoiTrangThai(string maSP, bool active);
}
