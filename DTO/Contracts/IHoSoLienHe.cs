namespace SmartShop.DTO.Contracts;

// Các thông tin chung phục vụ tìm kiếm và ràng buộc danh mục liên hệ.
public interface IHoSoLienHe
{
    string Code { get; }
    string Name { get; }
    string Phone { get; }
    string Email { get; }
}
