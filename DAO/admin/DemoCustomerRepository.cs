using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

/// <summary>Demo records scoped to one session. No SQL or production account is changed.</summary>
public sealed class DemoCustomerRepository : ICustomerRepository
{
    private readonly object gate = new();
    private readonly List<Customer> customers =
    [
        new("KH000001", "Nguyễn An", "0900000001", "an@example.com", "123 Đường Demo, TP. Hồ Chí Minh"),
        new("KH000002", "Trần Bình", "0900000002", "binh@example.com", "45 Đường Mẫu, TP. Hồ Chí Minh"),
        new("KH000003", "Lê Chi", "0900000003", "", "78 Đường Demo, TP. Hồ Chí Minh")
    ];

    public IReadOnlyList<Customer> GetAll()
    {
        lock (gate) return customers.ToArray();
    }

    public Customer? GetByCode(string code)
    {
        lock (gate) return customers.SingleOrDefault(value => value.Code == code);
    }

    public Customer Insert(Customer customer)
    {
        lock (gate)
        {
            EnsureUnique(customer.Code);
            customers.Add(customer);
            return customer;
        }
    }

    public Customer Update(Customer customer)
    {
        lock (gate)
        {
            var index = IndexOf(customer.Code);
            EnsureUnique(customer.Code, customer.Code);
            customers[index] = customer;
            return customer;
        }
    }

    public void Delete(string code)
    {
        lock (gate) customers.RemoveAt(IndexOf(code));
    }

    private int IndexOf(string code)
    {
        var index = customers.FindIndex(c => c.Code == code);
        return index >= 0 ? index : throw new InvalidOperationException("Không tìm thấy khách hàng.");
    }

    private void EnsureUnique(string code, string? exceptCode = null)
    {
        if (customers.Any(c => !string.Equals(c.Code, exceptCode, StringComparison.OrdinalIgnoreCase)
            && string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Mã khách hàng đã tồn tại.");
    }
}
