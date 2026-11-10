using SmartShop.DTO;

namespace SmartShop.BUS;

public interface IEmployeeBUS
{
    void Reload();
    IReadOnlyList<Employee> Search(string query = "", CatalogFilter? filter = null);
    Employee Get(string code);
    string SuggestCode();
    Employee Create(EmployeeDraft draft);
    Employee Update(string code, EmployeeDraft draft);
    void Delete(string code);
}
