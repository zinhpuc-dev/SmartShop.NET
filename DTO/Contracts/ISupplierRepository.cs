namespace SmartShop.DTO.Contracts;

public interface ISupplierRepository
{
    IReadOnlyList<Supplier> GetAll();
    Supplier? GetByCode(string code);
    Supplier Insert(Supplier supplier);
    Supplier Update(Supplier supplier);
    void Delete(string code);
}
