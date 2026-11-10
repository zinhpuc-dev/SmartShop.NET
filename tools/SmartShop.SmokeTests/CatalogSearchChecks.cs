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

    private static CatalogFilter Filter(params SearchCondition[] conditions) => new([new SearchGroup(conditions)]);

    private static void RejectFilter(Action action, string message)
    {
        try { action(); }
        catch (ArgumentException) { Check(true, message); return; }
        throw new Exception(message);
    }

    public static void RunAdvanced()
    {
        var data = new ProductData
        {
            Rows =
            [
                new("SP000001", "L00001", "Điện thoại Ánh Dương", "", "Chiếc", 120, 100.5m, "1") { TongSoLuongTon = 5 },
                new("SP000002", "L00001", "Điện thoại Sao Việt", "", "Chiếc", 60, 50, "0") { TongSoLuongTon = 0 },
                new("SP000003", "L00002", "Máy tính Đông Nam", "", "Chiếc", 220, 200, "1") { TongSoLuongTon = 20 }
            ]
        };
        var products = new ProductBUS(data);
        var comparisons = new[]
        {
            (SearchOperator.Equal, 1), (SearchOperator.NotEqual, 2), (SearchOperator.GreaterThan, 1),
            (SearchOperator.GreaterOrEqual, 2), (SearchOperator.LessThan, 1), (SearchOperator.LessOrEqual, 2)
        };
        foreach (var (operation, count) in comparisons)
            Check(products.Search("", null, Filter(new SearchCondition("Price", operation, "100.5"))).Count == count,
                "Numeric operator " + operation);
        Check(products.Search("", null, Filter(new SearchCondition("Price", SearchOperator.Equal, "100,5"))).Count == 1,
            "Vietnamese decimal separator");
        var combined = new CatalogFilter([
            new SearchGroup([
                new("Price", SearchOperator.GreaterThan, "50"),
                new("Stock", SearchOperator.LessOrEqual, "10"),
                new("Stock", SearchOperator.Equal, "0", true)]),
            new SearchGroup([new("Name", SearchOperator.Contains, "dong nam")])]);
        Check(products.Search("", null, combined).Select(value => value.MaSP).SequenceEqual(["SP000001", "SP000003"]),
            "AND groups joined with OR and NOT");
        Check(products.Search("dien thoai", "L00001", combined).Single().MaSP == "SP000001",
            "Advanced filter combines with existing query and category");
        Check(products.Search("", null, Filter(new SearchCondition("Name", SearchOperator.Contains, "dien thoai", true))).Single().MaSP == "SP000003",
            "Negated text condition");
        Check(products.Search("", null, Filter(new SearchCondition("Status", SearchOperator.Equal, "ngung ban"))).Single().MaSP == "SP000002",
            "Status uses accentless labels");
        Check(products.Search("", null, Filter(new SearchCondition("Category", SearchOperator.NotEqual, "L00001"))).Count == 1,
            "Text inequality");
        Check(products.Search("", null, new CatalogFilter([])).Count == 3, "Empty filter restores all rows");
        RejectFilter(() => products.Search("", null, Filter(new SearchCondition("Price", SearchOperator.Equal, "abc"))), "Reject invalid numeric input");
        RejectFilter(() => products.Search("", null, Filter(new SearchCondition("Price", SearchOperator.Equal, "1,000.5"))), "Reject grouped or ambiguous number");
        RejectFilter(() => products.Search("", null, Filter(new SearchCondition("Missing", SearchOperator.Equal, "x"))), "Reject unknown field");
        RejectFilter(() => products.Search("", null, Filter(new SearchCondition("Name", SearchOperator.GreaterThan, "x"))), "Reject numeric comparison on text");
        RejectFilter(() => products.Search("", null, Filter(new SearchCondition("Price", SearchOperator.Contains, "1"))), "Reject contains on numeric field");
        RejectFilter(() => products.Search("", null, Filter(new SearchCondition("Price", (SearchOperator)999, "1"))), "Reject invalid operator");
        RejectFilter(() => products.Search("", null, new CatalogFilter([new SearchGroup([])])), "Reject empty AND group");
        Check(data.Reads == 1 && data.Searches == 0, "Advanced search never rereads DAO while filtering");

        var empty = new ProductBUS(new ProductData { Rows = [] });
        RejectFilter(() => empty.Search("", null, Filter(new SearchCondition("Price", SearchOperator.Equal, "bad"))), "Validate filter even for empty catalog");
        Check(empty.Search("", null, combined).Count == 0, "Valid expression handles empty catalog");

        var customers = new CustomerBUS(new CustomerData());
        Check(customers.Search("", Filter(new SearchCondition("Name", SearchOperator.Equal, "nguyen an"),
            new("Email", SearchOperator.Equal, ""))).Count == 1, "Customer advanced search and empty email");
        var employees = new EmployeeBUS(new EmployeeData());
        Check(employees.Search("", Filter(new SearchCondition("Position", SearchOperator.Contains, "quan ly"))).Count == 1,
            "Employee advanced search by position");
        var suppliers = new SupplierBUS(new SupplierData());
        Check(suppliers.Search("", Filter(new SearchCondition("Name", SearchOperator.Contains, "anh duong"),
            new("Email", SearchOperator.Contains, "example"))).Count == 1, "Supplier multi-field advanced search");
        RejectFilter(() => suppliers.Search("", Filter(new SearchCondition("Stock", SearchOperator.Equal, "0"))), "Catalog field boundaries enforced in BUS");
    }

    public static void RunConstraints()
    {
        var repository = new CustomerData();
        var customers = new CustomerBUS(repository);
        var first = CustomerDraft.From(customers.Get("KH000001"));
        first.Email = "an@example.com";
        customers.Update(first.Code, first);
        var second = new CustomerDraft
        {
            Code = "KH000002", Name = "Trần Bình", Phone = "0900000002", Address = "123 Đường Demo",
            Email = "AN@example.com"
        };
        RejectWrite(() => customers.Create(second), "Customer duplicate email rejected by BUS");
        second.Email = "";
        customers.Create(second);
        var edit = CustomerDraft.From(customers.Get(second.Code));
        edit.Email = "an@example.com";
        RejectWrite(() => customers.Update(second.Code, edit), "Customer edit rejects duplicate email");
        Check(customers.Get(second.Code).Email == "", "Duplicate email preserves stored customer");
        edit.Code = "KH000009";
        RejectWrite(() => customers.Update(second.Code, edit), "Readonly customer code enforced by BUS");
        customers.Update(first.Code, first);
        Check(customers.Get(first.Code).Email == first.Email, "Customer can keep its own email");
        repository.FailWrites = true;
        first.Name = "Không lưu được";
        RejectWrite(() => customers.Update(first.Code, first), "Storage failure does not commit customer edit");
        Check(customers.Search("Không lưu được").Count == 0, "Failed customer edit preserves search cache");

        var employeeData = new EmployeeData();
        employeeData.Row = employeeData.Row with { Email = "minh@example.com" };
        var employees = new EmployeeBUS(employeeData);
        var duplicate = new EmployeeDraft
        {
            Code = "NV000002", Name = "Trần Hà", Position = "Bán hàng", Email = " MINH@example.com "
        };
        RejectWrite(() => employees.Create(duplicate), "Employee duplicate email rejected before DAO insert");
        var employeeEdit = EmployeeDraft.From(employeeData.Row);
        employees.Update(employeeEdit.Code, employeeEdit);
        Check(employees.Get(employeeEdit.Code).Email == employeeEdit.Email, "Employee can keep its own email");
        employeeEdit.Code = "NV000003";
        RejectWrite(() => employees.Update("NV000001", employeeEdit), "Readonly employee code enforced by BUS");

        var suppliers = new SupplierBUS(new SupplierData());
        var supplierEdit = SupplierDraft.From(suppliers.Get("N00001"));
        supplierEdit.Code = "N00002";
        RejectWrite(() => suppliers.Update("N00001", supplierEdit), "Readonly supplier code enforced by BUS");
    }

    private static void RejectWrite(Action action, string message)
    {
        try { action(); }
        catch (InvalidOperationException) { Check(true, message); return; }
        throw new Exception(message);
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
        public List<ProductManage>? Rows { get; init; }
        public List<ProductManage> ReadData() { Reads++; return (Rows ?? [Row]).Select(value => value with { }).ToList(); }
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
