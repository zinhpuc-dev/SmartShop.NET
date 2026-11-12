namespace SmartShop.DTO.Contracts;

public interface INhaCungCapDAO
{
    IReadOnlyList<NhaCungCapDTO> DocDanhSach();
    NhaCungCapDTO? DocTheoMa(string code);
    NhaCungCapDTO Them(NhaCungCapDTO supplier);
    NhaCungCapDTO Sua(NhaCungCapDTO supplier);
    void Xoa(string code);
}
