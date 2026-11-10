using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

var shop = new ShopState(new DemoShopDao());
static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine("PASS " + message);
}
static void Rejected(Action action, string message)
{
    try { action(); } catch (InvalidOperationException) { Console.WriteLine("PASS " + message); return; }
    throw new Exception(message);
}
var first = shop.Products[0];
Check(shop.Search("Samsung", "Điện thoại", null, "low").Count() == 2, "BUS filters the product catalog");
Check(shop.Add(first) && shop.Add(first), "Add two products");
Check(shop.Count == 2 && shop.Total == first.Price * 2, "Quantity and total are correct");
Rejected(() => shop.UpdateProduct(first.Id, first.Price, 1), "Stock cannot fall below cart quantity");
Rejected(() => shop.Checkout(" ", "0900000000", "123 Demo Street"), "Invalid delivery input is rejected by BUS");
Check(shop.Count == 2 && shop.Orders.Count == 0 && shop.Products[0].Stock == first.Stock, "Failed checkout leaves cart and stock unchanged");
var order = shop.Checkout("Demo Customer", "0900000000", "123 Demo Street");
Check(order.Total == first.Price * 2 && order.Items[0].Quantity == 2, "Order snapshot has correct totals");
Check(shop.Products[0].Stock == first.Stock - 2 && shop.Count == 0 && shop.Orders.Count == 1, "Checkout updates stock and empties cart");
shop.UpdateProduct(first.Id, first.Price + 1000, first.Stock - 2);
Check(order.Total == first.Price * 2 && order.Items[0].Product.Price == first.Price, "Changing catalog price preserves historical order price");
shop.UpdateOrderStatus(order.Code, "Đã xác nhận");
Check(shop.Orders[0].Status == "Đã xác nhận", "Admin updates order status through BUS");
Rejected(() => shop.UpdateOrderStatus(order.Code, "unknown"), "Invalid order status is rejected");
Rejected(() => shop.Checkout("Demo Customer", "0900000000", "123 Demo Street"), "Empty cart checkout is rejected");
var secondSession = new ShopState(new DemoShopDao());
Check(secondSession.Orders.Count == 0 && secondSession.Products[0].Stock == first.Stock, "Demo sessions are isolated");
var lastProduct = shop.Products[^1];
shop.UpdateProduct(lastProduct.Id, lastProduct.Price, 1);
Check(shop.Add(lastProduct) && !shop.Add(lastProduct), "Cart cannot exceed available stock");
// A replacement repository must be used by BUS without knowing a concrete DAO.
var rejectingRepository = new RejectingRepository();
IShopService substitutedService = new ShopState(rejectingRepository);
substitutedService.Add(rejectingRepository.GetProducts()[0]);
Rejected(() => substitutedService.Checkout("Demo Customer", "0900000000", "123 Demo Street"),
    "A substituted repository can reject persistence");
Check(rejectingRepository.CommitAttempts == 1 && substitutedService.Count == 1
    && substitutedService.Orders.Count == 0 && substitutedService.Products[0].Stock == 3,
    "Storage failure preserves cart and existing data");
Check(typeof(ShopState).Assembly.GetReferencedAssemblies().All(a => a.Name != "SmartShop.DAO"),
    "BUS has no assembly dependency on the concrete DAO");

var services = new ServiceCollection();
services.AddScoped<IShopRepository, DemoShopDao>();
services.AddScoped<IShopService, ShopState>();
using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
using var sessionA = provider.CreateScope();
using var sessionB = provider.CreateScope();
var serviceA = sessionA.ServiceProvider.GetRequiredService<IShopService>();
var serviceB = sessionB.ServiceProvider.GetRequiredService<IShopService>();
Check(ReferenceEquals(serviceA, sessionA.ServiceProvider.GetRequiredService<IShopService>()),
    "User and Admin share the same service within a scope");
serviceA.Add(serviceA.Products[0]);
Check(serviceA.Count == 1 && serviceB.Count == 0 && !ReferenceEquals(serviceA, serviceB),
    "DI scopes isolate customer session data");

CatalogSearchChecks.Run();
Console.WriteLine("All smoke checks passed.");

sealed class RejectingRepository : IShopRepository
{
    private readonly Product product = new(101, "Injected test product", "Điện thoại", "blue", 1000, 1200, 3, "Test");
    public int CommitAttempts { get; private set; }
    public IReadOnlyList<Product> GetProducts() => new[] { product };
    public ArrayList LoadProductArray() => new() { product };
    public IReadOnlyList<Order> GetOrders(string phone) => Array.Empty<Order>();
    public void UpdateProduct(Product value) => throw new NotSupportedException();
    public void UpdateOrderStatus(string code, string status) => throw new NotSupportedException();
    public Order CommitOrder(Order order)
    {
        CommitAttempts++;
        throw new InvalidOperationException("Simulated storage failure.");
    }
}
