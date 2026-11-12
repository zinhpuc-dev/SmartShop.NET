namespace SmartShop.DTO.Contracts;

public interface INhanVienDAO
{
    IReadOnlyList<NhanVienDTO> DocDanhSach();
    NhanVienDTO? DocTheoMa(string code);
    NhanVienDTO Them(NhanVienDTO employee);
    NhanVienDTO Sua(NhanVienDTO employee);
    void Xoa(string code);
}
