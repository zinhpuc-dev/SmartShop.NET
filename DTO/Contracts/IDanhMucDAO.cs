namespace SmartShop.DTO.Contracts;

public interface IDanhMucDAO<TDto> where TDto : class
{
    IReadOnlyList<TDto> DocDanhSach();
    TDto? DocTheoMa(string code);
    TDto Them(TDto value);
    TDto Sua(TDto value);
    void Xoa(string code);
}
