using System.Collections;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

// Per-session demo store. SQL Server storage can replace this DAO when the schema is ready.
public sealed class CuaHangGiaLapDAO : ICuaHangDAO
{
    private List<SanPhamDTO> products = [
        new(1,"uPhone 16 Pro Max","Điện thoại","titan",34990000,37990000,24,"256GB · Titan sa mạc"),
        new(2,"uPhone 16","Điện thoại","blue",21990000,24990000,38,"128GB · Xanh lưu ly"),
        new(3,"Samsung Galaxy S24 Ultra","Điện thoại","purple",27990000,33990000,16,"256GB · Xám Titan"),
        new(4,"MacBook Air M3","Laptop","silver",25990000,28990000,12,"16GB / 256GB · Bạc"),
        new(5,"uPad Air M2","Máy tính bảng","blue",14990000,16990000,18,"11 inch · 128GB · Xanh"),
        new(6,"AirPods Pro 2","Phụ kiện","silver",5490000,6190000,42,"USB-C · Chống ồn chủ động"),
        new(7,"Samsung Galaxy Z Flip6","Điện thoại","green",22990000,28990000,9,"256GB · Xanh bạc hà"),
        new(8,"MacBook Pro M3","Laptop","purple",38990000,42990000,8,"14 inch · 16GB / 512GB")
    ];
    private List<DonHangDTO> orders = [];
    private readonly object gate = new();

    public IReadOnlyList<SanPhamDTO> DocSanPham() => products.AsReadOnly();
    public ArrayList NapMangSanPham() => new(products);
    public IReadOnlyList<DonHangDTO> DocDonHang(string phone) => orders.Where(order => order.Phone == phone).ToList().AsReadOnly();

    public void CapNhatSanPham(SanPhamDTO value)
    {
        lock (gate)
        {
            var index = products.FindIndex(p => p.Id == value.Id);
            if (index < 0) throw new InvalidOperationException("Không tìm thấy sản phẩm.");
            products[index] = value;
        }
    }

    public void CapNhatTrangThaiDonHang(string code, string status)
    {
        lock (gate)
        {
            var index = orders.FindIndex(o => o.Code == code);
            if (index < 0) throw new InvalidOperationException("Không tìm thấy đơn hàng.");
            orders[index] = orders[index] with { Status = status };
        }
    }

    public DonHangDTO LuuDonHang(DonHangDTO order)
    {
        lock (gate)
        {
            if (orders.Any(o => o.Code == order.Code))
                throw new InvalidOperationException("Mã đơn hàng đã tồn tại.");
            // Prepare both snapshots first. Failed validation leaves the store unchanged.
            var nextProducts = products.ToList();
            foreach (var line in order.Items)
            {
                var index = nextProducts.FindIndex(p => p.Id == line.Product.Id);
                if (index < 0 || line.Quantity <= 0 || nextProducts[index].Stock < line.Quantity)
                    throw new InvalidOperationException("Không đủ tồn kho.");
                nextProducts[index] = nextProducts[index] with { Stock = nextProducts[index].Stock - line.Quantity };
            }
            var nextOrders = new List<DonHangDTO>(orders);
            nextOrders.Insert(0, order);
            products = nextProducts;
            orders = nextOrders;
            return order;
        }
    }
}
