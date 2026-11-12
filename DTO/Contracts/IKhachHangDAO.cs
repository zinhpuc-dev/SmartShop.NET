namespace SmartShop.DTO.Contracts;

public interface IKhachHangDAO
{
    IReadOnlyList<KhachHangDTO> DocDanhSach();
    KhachHangDTO? DocTheoMa(string code);
    KhachHangDTO Them(KhachHangDTO customer);
    KhachHangDTO Sua(KhachHangDTO customer);
    void Xoa(string code);
}
