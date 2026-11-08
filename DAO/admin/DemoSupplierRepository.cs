using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

/// <summary>Demo records scoped to one session. Uses in-memory storage.</summary>
public sealed class DemoSupplierRepository : ISupplierRepository
{
    private readonly object gate = new();
    private readonly List<Supplier> suppliers =
    [
        new("N00001", "Thiết Bị Ánh Dương", "02800000001", "anhduong@example.com", "123 Đường Demo, TP. Hồ Chí Minh"),
        new("N00002", "Linh Kiện Sao Việt", "02800000002", "saoviet@example.com", "45 Đường Mẫu, TP. Hồ Chí Minh"),
        new("N00003", "Công Nghệ Đông Nam", "02800000003", "", "78 Đường Demo, TP. Hồ Chí Minh")
    ];

    public IReadOnlyList<Supplier> GetAll()
    {
        lock (gate) return suppliers.ToArray();
    }

    public Supplier? GetByCode(string code)
    {
        lock (gate) return suppliers.SingleOrDefault(value => value.Code == code);
    }

    public Supplier Insert(Supplier supplier)
    {
        lock (gate)
        {
            EnsureUnique(supplier.Code);
            suppliers.Add(supplier);
            return supplier;
        }
    }

    public Supplier Update(Supplier supplier)
    {
        lock (gate)
        {
            var index = IndexOf(supplier.Code);
            EnsureUnique(supplier.Code, supplier.Code);
            suppliers[index] = supplier;
            return supplier;
        }
    }

    public void Delete(string code)
    {
        lock (gate) suppliers.RemoveAt(IndexOf(code));
    }

    private int IndexOf(string code)
    {
        var index = suppliers.FindIndex(c => c.Code == code);
        return index >= 0 ? index : throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");
    }

    private void EnsureUnique(string code, string? exceptCode = null)
    {
        if (suppliers.Any(c => !string.Equals(c.Code, exceptCode, StringComparison.OrdinalIgnoreCase)
            && string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Mã nhà cung cấp đã tồn tại.");
    }
}
