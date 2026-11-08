using System.Collections;
using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class MySqlShopDAO(string connectionString) : IShopRepository
{
    public IReadOnlyList<Product> GetProducts()
    {
        List<Product> products = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "SELECT p.MaSP,p.MaLoai,p.TenSP,p.MoTa,p.GiaNiemYet,p.GiaBan,CAST(COALESCE(SUM(t.SoLuongTon),0) AS SIGNED) AS Stock " +
            "FROM SAN_PHAM p LEFT JOIN TON_KHO t ON t.MaSP=p.MaSP WHERE p.TrangThai=1 " +
            "GROUP BY p.MaSP,p.MaLoai,p.TenSP,p.MoTa,p.GiaNiemYet,p.GiaBan ORDER BY p.MaSP";
        using MySqlCommand command = new(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();
        int id = 1;
        while (reader.Read())
        {
            string code = reader.GetString("MaSP").Trim();
            string categoryCode = reader.GetString("MaLoai").Trim();
            string description = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa");
            products.Add(new Product(id++, reader.GetString("TenSP"), "Sản phẩm", "silver",
                reader.GetDecimal("GiaBan"), reader.GetDecimal("GiaNiemYet"), reader.GetInt32("Stock"), description)
            {
                Code = code,
                CategoryCode = categoryCode
            });
        }
        return products;
    }

    public ArrayList LoadProductArray() => new(GetProducts().ToList());

    public IReadOnlyList<Order> GetOrders(string phone)
    {
        List<Order> orders = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "SELECT d.MaDH,c.MaKH,c.HoTen,c.SDT,d.DiaChiGiaoHang,d.NgayLap,d.TrangThai,d.TongTien " +
            "FROM DON_HANG d JOIN KHACH_HANG c ON c.MaKH=d.MaKH WHERE c.SDT=@Phone ORDER BY d.NgayLap DESC,d.MaDH";
        using (MySqlCommand command = new(query, connection))
        {
            command.Parameters.AddWithValue("@Phone", phone);
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                orders.Add(new Order(reader.GetString("MaDH").Trim(), reader.GetString("HoTen"), reader.GetString("SDT"),
                    reader.GetString("DiaChiGiaoHang"), reader.GetDateTime("NgayLap"), [], reader.GetDecimal("TongTien"))
                {
                    Status = reader.GetString("TrangThai"),
                    CustomerCode = reader.GetString("MaKH").Trim()
                });
        }
        for (int i = 0; i < orders.Count; i++)
        {
            Order order = orders[i];
            List<OrderLine> lines = [];
            using MySqlCommand command = new("SELECT ct.MaSP,p.TenSP,p.MaLoai,p.MoTa,p.GiaNiemYet,ct.DonGiaThucTe,ct.SoLuongDat,CAST(COALESCE(stock.Total,0) AS SIGNED) AS Stock " +
                "FROM CHI_TIET_DON_HANG ct JOIN SAN_PHAM p ON p.MaSP=ct.MaSP LEFT JOIN (SELECT MaSP,SUM(SoLuongTon) AS Total FROM TON_KHO GROUP BY MaSP) stock ON stock.MaSP=p.MaSP " +
                "WHERE ct.MaDH=@Code ORDER BY ct.MaSP", connection);
            command.Parameters.AddWithValue("@Code", order.Code);
            using MySqlDataReader reader = command.ExecuteReader();
            int id = 1;
            while (reader.Read())
            {
                string productCode = reader.GetString("MaSP").Trim();
                string categoryCode = reader.GetString("MaLoai").Trim();
                string spec = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa");
                Product product = new(id++, reader.GetString("TenSP"), "Sản phẩm", "silver", reader.GetDecimal("DonGiaThucTe"),
                    reader.GetDecimal("GiaNiemYet"), reader.GetInt32("Stock"), spec) { Code = productCode, CategoryCode = categoryCode };
                lines.Add(new OrderLine(product, reader.GetInt32("SoLuongDat")));
            }
            orders[i] = order with { Items = lines.AsReadOnly() };
        }
        return orders.AsReadOnly();
    }

    public void UpdateProduct(Product value) => throw new InvalidOperationException("Thông tin sản phẩm chỉ được cập nhật trong trang quản trị.");

    public void UpdateOrderStatus(string code, string status) => throw new InvalidOperationException("Trạng thái đơn hàng chỉ được cập nhật trong trang quản trị.");

    public Order CommitOrder(Order order)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlTransaction transaction = connection.BeginTransaction();
        try
        {
            string customerCode = FindOrCreateCustomer(connection, transaction, order);
            foreach (OrderLine line in order.Items)
            {
                using MySqlCommand stock = new("SELECT COALESCE(SUM(SoLuongTon),0) FROM TON_KHO WHERE MaSP=@Product FOR UPDATE", connection, transaction);
                stock.Parameters.AddWithValue("@Product", line.Product.Code);
                int available = Convert.ToInt32(stock.ExecuteScalar());
                if (line.Quantity <= 0 || available < line.Quantity)
                    throw new InvalidOperationException($"Không đủ tồn kho cho sản phẩm {line.Product.Name}.");
            }

            string orderCode = CreateOrderCode(connection, transaction);
            using (MySqlCommand header = new("INSERT INTO DON_HANG (MaDH,MaKH,NgayLap,TrangThai,DiaChiGiaoHang,TongTien) VALUES (@Code,@Customer,@Created,@Status,@Address,@Total)", connection, transaction))
            {
                header.Parameters.AddWithValue("@Code", orderCode);
                header.Parameters.AddWithValue("@Customer", customerCode);
                header.Parameters.AddWithValue("@Created", order.Created);
                header.Parameters.AddWithValue("@Status", "Đang xử lý");
                header.Parameters.AddWithValue("@Address", order.Address);
                header.Parameters.AddWithValue("@Total", order.Total);
                header.ExecuteNonQuery();
            }

            foreach (OrderLine line in order.Items)
            {
                using MySqlCommand detail = new("INSERT INTO CHI_TIET_DON_HANG (MaDH,MaSP,SoLuongDat,DonGia,TyLeGiamGia,DonGiaThucTe,ThanhTien) VALUES (@Order,@Product,@Quantity,@Price,0,@Price,@LineTotal)", connection, transaction);
                detail.Parameters.AddWithValue("@Order", orderCode);
                detail.Parameters.AddWithValue("@Product", line.Product.Code);
                detail.Parameters.AddWithValue("@Quantity", line.Quantity);
                detail.Parameters.AddWithValue("@Price", line.Product.Price);
                detail.Parameters.AddWithValue("@LineTotal", line.Product.Price * line.Quantity);
                detail.ExecuteNonQuery();
            }

            transaction.Commit();
            return order with { Code = orderCode, Status = "Đang xử lý", CustomerCode = customerCode };
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static string FindOrCreateCustomer(MySqlConnection connection, MySqlTransaction transaction, Order order)
    {
        using (MySqlCommand find = new("SELECT MaKH FROM KHACH_HANG WHERE SDT=@Phone ORDER BY MaKH LIMIT 1 FOR UPDATE", connection, transaction))
        {
            find.Parameters.AddWithValue("@Phone", order.Phone);
            object? existing = find.ExecuteScalar();
            if (existing is not null && existing != DBNull.Value) return Convert.ToString(existing)!.Trim();
        }

        for (int attempt = 0; attempt < 10; attempt++)
        {
            string code = $"KH{Guid.NewGuid():N}"[..8].ToUpperInvariant();
            using MySqlCommand insert = new("INSERT IGNORE INTO KHACH_HANG (MaKH,HoTen,SDT,DiaChi,Email) VALUES (@Code,@Name,@Phone,@Address,NULL)", connection, transaction);
            insert.Parameters.AddWithValue("@Code", code);
            insert.Parameters.AddWithValue("@Name", order.Customer);
            insert.Parameters.AddWithValue("@Phone", order.Phone);
            insert.Parameters.AddWithValue("@Address", order.Address);
            if (insert.ExecuteNonQuery() == 1) return code;
        }
        throw new InvalidOperationException("Không thể tạo thông tin khách hàng. Vui lòng thử lại.");
    }

    private static string CreateOrderCode(MySqlConnection connection, MySqlTransaction transaction)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            string code = $"DH{Guid.NewGuid():N}"[..8].ToUpperInvariant();
            using MySqlCommand check = new("SELECT COUNT(*) FROM DON_HANG WHERE MaDH=@Code", connection, transaction);
            check.Parameters.AddWithValue("@Code", code);
            if (Convert.ToInt32(check.ExecuteScalar()) == 0) return code;
        }
        throw new InvalidOperationException("Không thể tạo mã đơn hàng. Vui lòng thử lại.");
    }
}
