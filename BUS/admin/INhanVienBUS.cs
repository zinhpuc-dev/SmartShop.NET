using SmartShop.DTO;

namespace SmartShop.BUS;

public interface INhanVienBUS
{
    void TaiLai();
    IReadOnlyList<NhanVienDTO> TimKiem(string query = "", BoLocDanhMuc? filter = null);
    NhanVienDTO LayTheoMa(string code);
    string GoiYMa();
    NhanVienDTO Them(DuLieuNhanVien draft);
    NhanVienDTO Sua(string code, DuLieuNhanVien draft);
    void Xoa(string code);
}
