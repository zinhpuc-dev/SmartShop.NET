using SmartShop.DTO;

namespace SmartShop.BUS;

public interface IKhachHangBUS
{
    void TaiLai();
    IReadOnlyList<KhachHangDTO> TimKiem(string query = "", BoLocDanhMuc? filter = null);
    KhachHangDTO LayTheoMa(string code);
    string GoiYMa();
    KhachHangDTO Them(DuLieuKhachHang draft);
    KhachHangDTO Sua(string code, DuLieuKhachHang draft);
    void Xoa(string code);
}
