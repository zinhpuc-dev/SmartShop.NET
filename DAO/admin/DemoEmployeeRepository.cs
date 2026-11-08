using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

/// <summary>Demo records scoped to one session. No SQL or production account is changed.</summary>
public sealed class DemoEmployeeRepository : IEmployeeRepository
{
    private readonly object gate = new();
    private readonly List<Employee> employees =
    [
        new("NV000001", "Nguyễn Minh", "Nhân viên", "0910000001", "minh@example.com"),
        new("NV000002", "Trần Hà", "Nhân viên", "0910000002", "ha@example.com"),
        new("NV000003", "Lê Dũng", "Quản lý", "0910000003", "")
    ];

    public IReadOnlyList<Employee> GetAll()
    {
        lock (gate) return employees.ToArray();
    }

    public Employee? GetByCode(string code)
    {
        lock (gate) return employees.SingleOrDefault(value => value.Code == code);
    }

    public Employee Insert(Employee employee)
    {
        lock (gate)
        {
            EnsureUnique(employee.Code);
            employees.Add(employee);
            return employee;
        }
    }

    public Employee Update(Employee employee)
    {
        lock (gate)
        {
            var index = IndexOf(employee.Code);
            EnsureUnique(employee.Code, employee.Code);
            employees[index] = employee;
            return employee;
        }
    }

    public void Delete(string code)
    {
        lock (gate) employees.RemoveAt(IndexOf(code));
    }

    private int IndexOf(string code)
    {
        var index = employees.FindIndex(c => c.Code == code);
        return index >= 0 ? index : throw new InvalidOperationException("Không tìm thấy nhân viên.");
    }

    private void EnsureUnique(string code, string? exceptCode = null)
    {
        if (employees.Any(c => !string.Equals(c.Code, exceptCode, StringComparison.OrdinalIgnoreCase)
            && string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Mã nhân viên đã tồn tại.");
    }
}
