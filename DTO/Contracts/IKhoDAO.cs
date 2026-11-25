namespace SmartShop.DTO.Contracts;

public interface IKhoDAO
{
    IReadOnlyList<KhoDTO> DocDanhSach();
    KhoDTO? DocTheoMa(string code);
    void Them(KhoDTO warehouse);
    void Sua(KhoDTO warehouse);
    void Xoa(string code);
    IReadOnlyList<PhieuNhapDTO> DocPhieuNhap();
    void LapPhieuNhap(DuLieuPhieuNhap draft);
    IReadOnlyList<ChiTietPhieuNhapDTO> DocChiTietPhieuNhap(string code);
    IReadOnlyList<TonKhoDTO> DocTonKho();
}
