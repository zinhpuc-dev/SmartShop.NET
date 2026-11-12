using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

var shop = new CuaHangBUS(new CuaHangGiaLapDAO());
static void KiemTra(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine("PASS " + message);
}
static void KiemTraTuChoi(Action action, string message)
{
    try { action(); } catch (InvalidOperationException) { Console.WriteLine("PASS " + message); return; }
    throw new Exception(message);
}
var first = shop.Products[0];
KiemTra(shop.TimKiem("Samsung", "Điện thoại", null, "low").Count() == 2, "BUS filters the product catalog");
KiemTra(shop.ThemVaoGio(first) && shop.ThemVaoGio(first), "Add two products");
KiemTra(shop.Count == 2 && shop.Total == first.Price * 2, "Quantity and total are correct");
KiemTraTuChoi(() => shop.CapNhatSanPham(first.Id, first.Price, 1), "Stock cannot fall below cart quantity");
KiemTraTuChoi(() => shop.ThanhToan(" ", "0900000000", "123 Demo Street"), "Invalid delivery input is rejected by BUS");
KiemTra(shop.Count == 2 && shop.Orders.Count == 0 && shop.Products[0].Stock == first.Stock, "Failed checkout leaves cart and stock unchanged");
var order = shop.ThanhToan("Demo KhachHangDTO", "0900000000", "123 Demo Street");
KiemTra(order.Total == first.Price * 2 && order.Items[0].Quantity == 2, "Order snapshot has correct totals");
KiemTra(shop.Products[0].Stock == first.Stock - 2 && shop.Count == 0 && shop.Orders.Count == 1, "Checkout updates stock and empties cart");
shop.CapNhatSanPham(first.Id, first.Price + 1000, first.Stock - 2);
KiemTra(order.Total == first.Price * 2 && order.Items[0].Product.Price == first.Price, "Changing catalog price preserves historical order price");
shop.CapNhatTrangThaiDonHang(order.Code, "Đã xác nhận");
KiemTra(shop.Orders[0].Status == "Đã xác nhận", "Admin updates order status through BUS");
KiemTraTuChoi(() => shop.CapNhatTrangThaiDonHang(order.Code, "unknown"), "Invalid order status is rejected");
KiemTraTuChoi(() => shop.ThanhToan("Demo KhachHangDTO", "0900000000", "123 Demo Street"), "Empty cart checkout is rejected");
var secondSession = new CuaHangBUS(new CuaHangGiaLapDAO());
KiemTra(secondSession.Orders.Count == 0 && secondSession.Products[0].Stock == first.Stock, "Demo sessions are isolated");
var lastProduct = shop.Products[^1];
shop.CapNhatSanPham(lastProduct.Id, lastProduct.Price, 1);
KiemTra(shop.ThemVaoGio(lastProduct) && !shop.ThemVaoGio(lastProduct), "Cart cannot exceed available stock");
// A replacement repository must be used by BUS without knowing a concrete DAO.
var rejectingRepository = new KhoDuLieuTuChoi();
ICuaHangBUS substitutedService = new CuaHangBUS(rejectingRepository);
substitutedService.ThemVaoGio(rejectingRepository.DocSanPham()[0]);
KiemTraTuChoi(() => substitutedService.ThanhToan("Demo KhachHangDTO", "0900000000", "123 Demo Street"),
    "A substituted repository can reject persistence");
KiemTra(rejectingRepository.CommitAttempts == 1 && substitutedService.Count == 1
    && substitutedService.Orders.Count == 0 && substitutedService.Products[0].Stock == 3,
    "Storage failure preserves cart and existing data");
KiemTra(typeof(CuaHangBUS).Assembly.GetReferencedAssemblies().All(a => a.Name != "SmartShop.DAO"),
    "BUS has no assembly dependency on the concrete DAO");

var services = new ServiceCollection();
services.AddScoped<ICuaHangDAO, CuaHangGiaLapDAO>();
services.AddScoped<ICuaHangBUS, CuaHangBUS>();
using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
using var sessionA = provider.CreateScope();
using var sessionB = provider.CreateScope();
var serviceA = sessionA.ServiceProvider.GetRequiredService<ICuaHangBUS>();
var serviceB = sessionB.ServiceProvider.GetRequiredService<ICuaHangBUS>();
KiemTra(ReferenceEquals(serviceA, sessionA.ServiceProvider.GetRequiredService<ICuaHangBUS>()),
    "User and Admin share the same service within a scope");
serviceA.ThemVaoGio(serviceA.Products[0]);
KiemTra(serviceA.Count == 1 && serviceB.Count == 0 && !ReferenceEquals(serviceA, serviceB),
    "DI scopes isolate customer session data");

KiemTraTimKiemDanhMuc.Chay();
KiemTraTimKiemDanhMuc.ChayTimKiemNangCao();
KiemTraTimKiemDanhMuc.ChayKiemTraRangBuoc();
Console.WriteLine("All smoke checks passed.");

sealed class KhoDuLieuTuChoi : ICuaHangDAO
{
    private readonly SanPhamDTO product = new(101, "Injected test product", "Điện thoại", "blue", 1000, 1200, 3, "Test");
    public int CommitAttempts { get; private set; }
    public IReadOnlyList<SanPhamDTO> DocSanPham() => new[] { product };
    public ArrayList NapMangSanPham() => new() { product };
    public IReadOnlyList<DonHangDTO> DocDonHang(string phone) => Array.Empty<DonHangDTO>();
    public void CapNhatSanPham(SanPhamDTO value) => throw new NotSupportedException();
    public void CapNhatTrangThaiDonHang(string code, string status) => throw new NotSupportedException();
    public DonHangDTO LuuDonHang(DonHangDTO order)
    {
        CommitAttempts++;
        throw new InvalidOperationException("Simulated storage failure.");
    }
}
