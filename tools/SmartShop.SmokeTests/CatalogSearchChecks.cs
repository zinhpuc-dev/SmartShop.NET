using SmartShop.BUS;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class CatalogSearchChecks
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS SEARCH " + message);
    }

    public static void Run()
    {
        var customerData = new CustomerData();
        var customers = new CustomerBUS(customerData);
        Check(customers.Search("  nguyen an ").Single().Code == "KH000001", "Customer accentless search");
        customers.Search("090"); customers.Search(""); customers.SuggestCode();
        Check(customerData.Reads == 1, "Customer search loads once");
        var added = customers.Create(new CustomerDraft
        {
            Code = "KH000002", Name = "Trần Bình", Phone = "0900000002", Address = "123 Đường Demo"
        });
        Check(customers.Search("tran binh").Single() == added, "Customer create updates cache");
        var draft = CustomerDraft.From(added);
        draft.Name = "Lê Chi";
        customers.Update(added.Code, draft);
        Check(customers.Search("le chi").Count == 1 && customers.Search("tran binh").Count == 0,
            "Customer edit updates cache");
        customerData.FailWrites = true;
        try { customers.Delete(added.Code); throw new Exception("Delete should fail"); }
        catch (InvalidOperationException) { }
        Check(customers.Search("le chi").Count == 1, "Failed write preserves cache");
        customerData.FailWrites = false;
        customers.Delete(added.Code);
        Check(customers.Search("le chi").Count == 0, "Customer delete updates cache");
        customers.Reload(); customers.Search(""); customers.Search("");
        Check(customerData.Reads == 2, "Explicit reload reads once");

        var employeeData = new EmployeeData();
        var employees = new EmployeeBUS(employeeData);
        Check(employees.Search("quan ly").Single().Code == "NV000001", "Employee searches position without accents");
        employees.Search("minh"); employees.Search("");
        Check(employeeData.Reads == 1, "Employee search loads once");
        var employeeDraft = EmployeeDraft.From(employeeData.Row);
        employeeDraft.Name = "Đỗ Hà";
        employees.Update(employeeDraft.Code, employeeDraft);
        Check(employees.Search("do ha").Count == 1, "Employee edit updates cache");
        employees.Delete(employeeDraft.Code);
        Check(employees.Search("").Count == 0, "Employee delete updates cache");

        var supplierData = new SupplierData();
        var suppliers = new SupplierBUS(supplierData);
        Check(suppliers.Search("anh duong").Single().Code == "N00001", "Supplier accentless search");
        suppliers.Search(""); suppliers.Search("example");
        Check(supplierData.Reads == 1, "Supplier search loads once");
        supplierData.Row = supplierData.Row with { Name = "Tên đổi bởi phiên khác" };
        var supplierDraft = SupplierDraft.From(supplierData.Row);
        supplierDraft.Name = "Đông Nam";
        suppliers.Update(supplierDraft.Code, supplierDraft);
        Check(suppliers.Search("dong nam").Count == 1, "Supplier replaces cache by code after external edit");
        suppliers.Delete(supplierDraft.Code);
        Check(suppliers.Search("").Count == 0, "Supplier delete updates cache");

        var productData = new ProductData();
        var products = new ProductBUS(productData);
        Check(products.Search("dien thoai").Count == 1, "Product accentless search");
        Check(products.Search("SP000001", "L00001").Count == 1
            && products.Search("SP000001", "L00002").Count == 0, "Product code and category filter");
        products.Search(""); products.Search("missing");
        Check(productData.Reads == 1 && productData.Searches == 0, "Product search stays in BUS");
        var result = products.Search("").Single();
        result.TenSP = "Changed result";
        Check(products.Search("dien thoai").Count == 1, "Mutable result cannot alter product cache");
        products.SetActive("SP000001", false);
        Check(products.Search("").Single().TrangThai == "0", "Product status refreshes cache");
        productData.Row = productData.Row with { TenSP = "Máy tính" };
        products.Read();
        Check(products.Search("may tinh").Count == 1, "Product explicit read refreshes cache");
    }

    private sealed class CustomerData : ICustomerRepository
    {
        private readonly List<Customer> rows = [new("KH000001", "Nguyễn An", "0900000001", "", "123 Đường Demo")];
        public int Reads { get; private set; }
        public bool FailWrites { get; set; }
        public IReadOnlyList<Customer> GetAll() { Reads++; return rows.ToArray(); }
        public Customer? GetByCode(string code) => rows.SingleOrDefault(row => row.Code == code);
        public Customer Insert(Customer value) { Verify(); rows.Add(value); return value; }
        public Customer Update(Customer value)
        {
            Verify();
            rows[rows.FindIndex(row => row.Code == value.Code)] = value;
            return value;
        }
        public void Delete(string code) { Verify(); rows.RemoveAll(row => row.Code == code); }
        private void Verify()
        {
            if (FailWrites) throw new InvalidOperationException("Simulated write failure");
        }
    }

    private sealed class EmployeeData : IEmployeeRepository
    {
        public Employee Row = new("NV000001", "Nguyễn Minh", "Quản lý", "", "");
        private bool deleted;
        public int Reads { get; private set; }
        public IReadOnlyList<Employee> GetAll() { Reads++; return deleted ? [] : [Row]; }
        public Employee? GetByCode(string code) => deleted ? null : Row;
        public Employee Insert(Employee value) => throw new NotSupportedException();
        public Employee Update(Employee value) => Row = value;
        public void Delete(string code) => deleted = true;
    }

    private sealed class SupplierData : ISupplierRepository
    {
        public Supplier Row = new("N00001", "Ánh Dương", "", "demo@example.com", "");
        private bool deleted;
        public int Reads { get; private set; }
        public IReadOnlyList<Supplier> GetAll() { Reads++; return deleted ? [] : [Row]; }
        public Supplier? GetByCode(string code) => deleted ? null : Row;
        public Supplier Insert(Supplier value) => throw new NotSupportedException();
        public Supplier Update(Supplier value) => Row = value;
        public void Delete(string code) => deleted = true;
    }

    private sealed class ProductData : IProductManage
    {
        public ProductManage Row = new("SP000001", "L00001", "Điện thoại", "", "Chiếc", 100, 90, "1");
        public int Reads { get; private set; }
        public int Searches { get; private set; }
        public List<ProductManage> ReadData() { Reads++; return [Row with { }]; }
        public List<ProductManage> Search(string keyword) { Searches++; throw new Exception("DAO search must not run"); }
        public ProductManage? GetByCode(string code) => Row with { };
        public bool CheckProduct(string code) => code == Row.MaSP;
        public void SetActive(string code, bool active) => Row.TrangThai = active ? "1" : "0";
        public List<ProductCategory> ReadCategories() => [];
        public bool CheckCategory(string code) => true;
        public void InsertCategory(ProductCategory value) => throw new NotSupportedException();
        public void UpdateCategory(ProductCategory value) => throw new NotSupportedException();
        public void DeleteCategory(string code) => throw new NotSupportedException();
        public List<ProductStock> ReadStockByProduct(string code) => [];
        public void Insert(ProductManage value) => throw new NotSupportedException();
        public void Update(ProductManage value) => Row = value with { };
        public void Delete(string code) => throw new NotSupportedException();
    }
}
