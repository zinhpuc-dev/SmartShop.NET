using System.Globalization;
using SmartShop.DTO;

namespace SmartShop.BUS;

// Dùng chung định nghĩa trường để giao diện và BUS kiểm tra cùng một bộ điều kiện.
public static class AdvancedCatalogSearch
{
    private static readonly SearchColumn<Customer>[] customerColumns =
    [
        Text<Customer>("Code", "Mã khách hàng", value => value.Code),
        Text<Customer>("Name", "Họ tên", value => value.Name),
        Text<Customer>("Phone", "Điện thoại", value => value.Phone),
        Text<Customer>("Email", "Email", value => value.Email),
        Text<Customer>("Address", "Địa chỉ", value => value.Address)
    ];
    private static readonly SearchColumn<Employee>[] employeeColumns =
    [
        Text<Employee>("Code", "Mã nhân viên", value => value.Code),
        Text<Employee>("Name", "Họ tên", value => value.Name),
        Text<Employee>("Position", "Chức vụ", value => value.Position),
        Text<Employee>("Phone", "Điện thoại", value => value.Phone),
        Text<Employee>("Email", "Email", value => value.Email)
    ];
    private static readonly SearchColumn<Supplier>[] supplierColumns =
    [
        Text<Supplier>("Code", "Mã nhà cung cấp", value => value.Code),
        Text<Supplier>("Name", "Tên nhà cung cấp", value => value.Name),
        Text<Supplier>("Phone", "Điện thoại", value => value.Phone),
        Text<Supplier>("Email", "Email", value => value.Email),
        Text<Supplier>("Address", "Địa chỉ", value => value.Address)
    ];
    private static readonly SearchColumn<ProductManage>[] productColumns =
    [
        Text<ProductManage>("Code", "Mã sản phẩm", value => value.MaSP),
        Text<ProductManage>("Name", "Tên sản phẩm", value => value.TenSP),
        Text<ProductManage>("Category", "Mã loại", value => value.MaLoai),
        Text<ProductManage>("Unit", "Đơn vị tính", value => value.DonViTinh),
        Number<ProductManage>("Price", "Giá bán", value => value.GiaBan),
        Number<ProductManage>("ListPrice", "Giá niêm yết", value => value.GiaNiemYet),
        Number<ProductManage>("Stock", "Tồn kho", value => value.TongSoLuongTon),
        Text<ProductManage>("Status", "Trạng thái", value => value.TrangThai == "1" ? "Đang bán" : "Ngừng bán")
    ];

    public static IReadOnlyList<SearchField> CustomerFields { get; } = Fields(customerColumns);
    public static IReadOnlyList<SearchField> EmployeeFields { get; } = Fields(employeeColumns);
    public static IReadOnlyList<SearchField> SupplierFields { get; } = Fields(supplierColumns);
    public static IReadOnlyList<SearchField> ProductFields { get; } = Fields(productColumns);

    internal static Func<Customer, bool> Customers(CatalogFilter? filter) => Compile(filter, customerColumns);
    internal static Func<Employee, bool> Employees(CatalogFilter? filter) => Compile(filter, employeeColumns);
    internal static Func<Supplier, bool> Suppliers(CatalogFilter? filter) => Compile(filter, supplierColumns);
    internal static Func<ProductManage, bool> Products(CatalogFilter? filter) => Compile(filter, productColumns);

    private static IReadOnlyList<SearchField> Fields<T>(SearchColumn<T>[] columns) =>
        Array.AsReadOnly(columns.Select(column => column.Field).ToArray());

    private static SearchColumn<T> Text<T>(string key, string label, Func<T, string> select) =>
        new(new SearchField(key, label), select, null);

    private static SearchColumn<T> Number<T>(string key, string label, Func<T, decimal> select) =>
        new(new SearchField(key, label, SearchFieldType.Number), null, select);

    private static Func<T, bool> Compile<T>(CatalogFilter? filter, SearchColumn<T>[] columns)
    {
        if (filter is null) return _ => true;
        if (filter.Groups is null || filter.Groups.Count > 8)
            throw new ArgumentException("Bộ lọc hỗ trợ tối đa 8 nhóm OR.");
        if (filter.Groups.Count == 0) return _ => true;

        var groups = new List<Func<T, bool>[]>();
        foreach (var group in filter.Groups)
        {
            if (group?.Conditions is null || group.Conditions.Count is 0 or > 8)
                throw new ArgumentException("Mỗi nhóm cần từ 1 đến 8 điều kiện AND.");
            groups.Add(group.Conditions.Select(condition => Condition(condition, columns)).ToArray());
        }
        // Kiểm tra toàn bộ điều kiện trước khi lọc, kể cả khi danh mục đang rỗng.
        return value => groups.Any(group => group.All(matches => matches(value)));
    }

    private static Func<T, bool> Condition<T>(SearchCondition condition, SearchColumn<T>[] columns)
    {
        if (condition is null || !Enum.IsDefined(condition.Operator))
            throw new ArgumentException("Phép so sánh không hợp lệ.");
        var column = columns.SingleOrDefault(value => value.Field.Key == condition.Field)
            ?? throw new ArgumentException("Trường tìm kiếm không hợp lệ cho danh mục này.");
        if (condition.Value is null || condition.Value.Length > 500)
            throw new ArgumentException("Giá trị tìm kiếm tối đa 500 ký tự.");

        Func<T, bool> matches;
        if (column.Number is not null)
        {
            if (condition.Operator == SearchOperator.Contains)
                throw new ArgumentException($"{column.Field.Label} cần phép so sánh số.");
            var input = condition.Value.Trim().Replace(',', '.');
            if (!decimal.TryParse(input, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var number))
                throw new ArgumentException($"{column.Field.Label} cần một số hợp lệ, không dùng dấu phân cách hàng nghìn.");
            matches = value => Compare(column.Number(value).CompareTo(number), condition.Operator);
        }
        else
        {
            if (condition.Operator is not (SearchOperator.Contains or SearchOperator.Equal or SearchOperator.NotEqual))
                throw new ArgumentException($"{column.Field.Label} chỉ hỗ trợ Chứa, = và <>.");
            var term = SearchText.Normalize(condition.Value.Trim());
            matches = value => MatchText(SearchText.Normalize(column.Text!(value) ?? ""), term, condition.Operator);
        }
        return condition.Negate ? value => !matches(value) : matches;
    }

    private static bool MatchText(string value, string term, SearchOperator operation) => operation switch
    {
        SearchOperator.Contains => value.Contains(term, StringComparison.OrdinalIgnoreCase),
        SearchOperator.Equal => value.Equals(term, StringComparison.OrdinalIgnoreCase),
        SearchOperator.NotEqual => !value.Equals(term, StringComparison.OrdinalIgnoreCase),
        _ => throw new ArgumentException("Phép so sánh văn bản không hợp lệ.")
    };

    private static bool Compare(int result, SearchOperator operation) => operation switch
    {
        SearchOperator.Equal => result == 0,
        SearchOperator.NotEqual => result != 0,
        SearchOperator.GreaterThan => result > 0,
        SearchOperator.GreaterOrEqual => result >= 0,
        SearchOperator.LessThan => result < 0,
        SearchOperator.LessOrEqual => result <= 0,
        _ => throw new ArgumentException("Phép so sánh số không hợp lệ.")
    };

    private sealed record SearchColumn<T>(SearchField Field, Func<T, string>? Text, Func<T, decimal>? Number);
}
