using SmartShop.DTO;

namespace SmartShop.BUS;

public interface IEmployeeService
{
    IReadOnlyList<Employee> Search(string query = "");
    Employee Get(string code);
    string SuggestCode();
    Employee Create(EmployeeDraft draft);
    Employee Update(string code, EmployeeDraft draft);
    void Delete(string code);
}
