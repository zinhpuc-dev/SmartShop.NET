namespace SmartShop.DTO.Contracts;

public interface IKhuyenMaiDAO
{
    IReadOnlyList<KhuyenMaiDTO> DocKhuyenMai();
    void ThemKhuyenMai(DuLieuKhuyenMai draft);
    void SuaKhuyenMai(DuLieuKhuyenMai draft);
    IReadOnlyList<MaGiamGiaDTO> DocMaGiamGia();
    MaGiamGiaDTO? DocMaGiamGiaTheoMa(string code);
    void ThemMaGiamGia(DuLieuMaGiamGia draft);
    void SuaMaGiamGia(DuLieuMaGiamGia draft);
}
