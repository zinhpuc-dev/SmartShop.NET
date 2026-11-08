using SmartShop.DTO;

namespace SmartShop.DTO.Contracts;

public interface IProductManage
{
    List<ProductManage> ReadData();
    List<ProductManage> Search(string keyword);
    ProductManage? GetByCode(string maSP);
    List<ProductCategory> ReadCategories();
    bool CheckCategory(string maLoai);
    void InsertCategory(ProductCategory category);
    void UpdateCategory(ProductCategory category);
    void DeleteCategory(string maLoai);
    List<ProductStock> ReadStockByProduct(string maSP);
    bool CheckProduct(string maSP);
    void Insert(ProductManage product);
    void Update(ProductManage product);
    void Delete(string maSP);
}
