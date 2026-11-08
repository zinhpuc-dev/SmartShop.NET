using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

// The GUI calls this service; only BUS talks to DAO.
public sealed class ShopState : IShopService
{
    private readonly IShopRepository dao;
    private readonly Dictionary<int, int> cart = [];
    private readonly IReadOnlyDictionary<int, int> cartView;
    private string customerPhone = "";

    public ShopState(IShopRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        dao = repository;
        cartView = new ReadOnlyDictionary<int, int>(cart);
    }

    public IReadOnlyList<Product> Products => dao.GetProducts();
    public IReadOnlyList<Order> Orders => string.IsNullOrWhiteSpace(customerPhone) ? [] : dao.GetOrders(customerPhone);
    public IReadOnlyDictionary<int, int> Cart => cartView;
    public int Count => cart.Values.Sum();
    public decimal Total => cart.Sum(x => FindProduct(x.Key).Price * x.Value);

    public IEnumerable<Product> Search(string query, string category, HashSet<int>? favorites, string sort)
    {
        // Filtering stays in BUS and works on an ArrayList loaded from DAO.
        var products = dao.LoadProductArray().Cast<Product>().Where(p =>
            (category == "Tất cả" || p.Category == category)
            && (p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                || p.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
            && (favorites is null || favorites.Contains(p.Id)));
        return sort switch
        {
            "low" => products.OrderBy(p => p.Price),
            "high" => products.OrderByDescending(p => p.Price),
            _ => products
        };
    }

    public bool Add(Product product)
    {
        var current = FindProduct(product.Id);
        if (cart.GetValueOrDefault(current.Id) >= current.Stock) return false;
        cart[current.Id] = cart.GetValueOrDefault(current.Id) + 1;
        return true;
    }
    public void Change(int id, int delta)
    {
        var product = FindProduct(id);
        var quantity = cart.GetValueOrDefault(id) + delta;
        if (quantity <= 0) cart.Remove(id);
        else if (quantity <= product.Stock) cart[id] = quantity;
    }
    public void Remove(int id) => cart.Remove(id);

    public void UpdateProduct(int id, decimal price, int stock)
    {
        if (price <= 0 || stock < 0 || stock < cart.GetValueOrDefault(id))
            throw new InvalidOperationException("Kiểm tra giá và tồn kho (không thấp hơn số lượng trong giỏ).");
        dao.UpdateProduct(FindProduct(id) with { Price = price, Stock = stock });
    }
    public void UpdateOrderStatus(string code, string status)
    {
        if (status is not ("Chờ xác nhận" or "Đã xác nhận" or "Đang giao hàng" or "Hoàn thành"))
            throw new InvalidOperationException("Trạng thái đơn hàng không hợp lệ.");
        dao.UpdateOrderStatus(code, status);
    }
    public Order Checkout(string name, string phone, string address)
    {
        if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(phone, @"^0[0-9]{9}$")
            || string.IsNullOrWhiteSpace(address) || address.Trim().Length < 10)
            throw new InvalidOperationException("Vui lòng kiểm tra thông tin nhận hàng.");
        if (cart.Count == 0) throw new InvalidOperationException("Giỏ hàng đang trống.");
        var lines = cart.Select(x => new OrderLine(FindProduct(x.Key), x.Value)).ToList().AsReadOnly();
        if (lines.Any(x => x.Quantity <= 0 || x.Quantity > x.Product.Stock))
            throw new InvalidOperationException("Không đủ tồn kho.");
        var order = new Order($"DH{Guid.NewGuid():N}"[..8].ToUpperInvariant(),
            name.Trim(), phone, address.Trim(), DateTime.Now, lines,
            lines.Sum(x => x.Product.Price * x.Quantity));
        order = dao.CommitOrder(order);
        customerPhone = phone.Trim();
        cart.Clear();
        return order;
    }
    private Product FindProduct(int id) => Products.SingleOrDefault(p => p.Id == id)
        ?? throw new InvalidOperationException("Không tìm thấy sản phẩm.");
}
