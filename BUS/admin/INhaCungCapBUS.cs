using SmartShop.DTO;

namespace SmartShop.BUS;

public interface INhaCungCapBUS
{
    void TaiLai();
    IReadOnlyList<NhaCungCapDTO> TimKiem(string query = "", BoLocDanhMuc? filter = null);
    NhaCungCapDTO LayTheoMa(string code);
    string GoiYMa();
    NhaCungCapDTO Them(DuLieuNhaCungCap draft);
    NhaCungCapDTO Sua(string code, DuLieuNhaCungCap draft);
    void Xoa(string code);
}
