namespace SmartShop.DTO.Contracts;

public interface IEmployeeRepository
{
    IReadOnlyList<Employee> GetAll();
    Employee? GetByCode(string code);
    Employee Insert(Employee employee);
    Employee Update(Employee employee);
    void Delete(string code);
}
