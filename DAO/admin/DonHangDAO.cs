using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class DonHangDAO(string connectionString) : IDonHangDAO
{
    public IReadOnlyList<KhoDTO> DocKho()
    {
        List<KhoDTO> warehouses = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaKho, TenKho FROM KHO ORDER BY MaKho", connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) warehouses.Add(new KhoDTO { Code = reader.GetString("MaKho").Trim(), Name = reader.GetString("TenKho") });
        return warehouses;
    }

    public IReadOnlyList<DonHangQuanTriDTO> DocDonHang()
    {
        List<DonHangQuanTriDTO> orders = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand schema = new("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='DON_HANG' AND COLUMN_NAME='MaKho'", connection);
        bool hasWarehouse = Convert.ToInt32(schema.ExecuteScalar()) == 1;
        string warehouseColumn = hasWarehouse ? "d.MaKho" : "NULL";
        string query = "SELECT d.MaDH,d.MaKH,k.HoTen,d.NgayLap,d.TrangThai,d.DiaChiGiaoHang,d.TongTien," + warehouseColumn + " AS MaKho," +
            "h.MaHoaDon,h.MaNV,n.TenNV FROM DON_HANG d JOIN KHACH_HANG k ON k.MaKH=d.MaKH " +
            "LEFT JOIN HOA_DON h ON h.MaDH=d.MaDH LEFT JOIN NHAN_VIEN n ON n.MaNV=h.MaNV ORDER BY d.NgayLap DESC,d.MaDH";
        using (MySqlCommand command = new(query, connection))
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string code = reader.GetString("MaDH").Trim();
                orders.Add(new DonHangQuanTriDTO(code, reader.GetString("MaKH").Trim(), reader.GetString("HoTen"), reader.GetDateTime("NgayLap"),
                    reader.GetString("TrangThai"), reader.GetString("DiaChiGiaoHang"), reader.GetDecimal("TongTien"),
                    reader.IsDBNull(reader.GetOrdinal("MaKho")) ? null : reader.GetString("MaKho").Trim(),
                    reader.IsDBNull(reader.GetOrdinal("MaHoaDon")) ? null : reader.GetString("MaHoaDon").Trim(),
                    reader.IsDBNull(reader.GetOrdinal("MaNV")) ? null : reader.GetString("MaNV").Trim(),
                    reader.IsDBNull(reader.GetOrdinal("TenNV")) ? null : reader.GetString("TenNV"), []));
            }
        }
        for (int index = 0; index < orders.Count; index++)
        {
            DonHangQuanTriDTO order = orders[index];
            using MySqlCommand lines = new("SELECT ct.MaSP,p.TenSP,ct.SoLuongDat,ct.DonGia,ct.DonGiaThucTe,ct.ThanhTien FROM CHI_TIET_DON_HANG ct JOIN SAN_PHAM p ON p.MaSP=ct.MaSP WHERE ct.MaDH=@Code ORDER BY ct.MaSP", connection);
            lines.Parameters.AddWithValue("@Code", order.Code);
            using MySqlDataReader lineReader = lines.ExecuteReader();
            List<ChiTietDonHangQuanTriDTO> items = [];
            while (lineReader.Read()) items.Add(new ChiTietDonHangQuanTriDTO(lineReader.GetString("MaSP").Trim(), lineReader.GetString("TenSP"),
                lineReader.GetInt32("SoLuongDat"), lineReader.GetDecimal("DonGia"), lineReader.GetDecimal("DonGiaThucTe"), lineReader.GetDecimal("ThanhTien")));
            orders[index] = order with { Lines = items };
        }
        return orders;
    }

    public void CapNhatTrangThai(string orderCode, string status, string warehouseCode)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlTransaction transaction = connection.BeginTransaction();
        try
        {
            bool warehouseAllocationAvailable;
            using (MySqlCommand schemaCheck = new("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='DON_HANG' AND COLUMN_NAME='MaKho'", connection, transaction))
                warehouseAllocationAvailable = Convert.ToInt32(schemaCheck.ExecuteScalar()) == 1;

            if (status is "Đã giao" or "Hoàn thành" && !warehouseAllocationAvailable)
                throw new InvalidOperationException("Để hoàn tất đơn và trừ tồn kho, cần duyệt và áp dụng migration phân bổ kho trước.");

            string? oldStatus;
            string? oldWarehouse;
            string statusQuery = warehouseAllocationAvailable
                ? "SELECT TrangThai,MaKho FROM DON_HANG WHERE MaDH=@Code FOR UPDATE"
                : "SELECT TrangThai,NULL AS MaKho FROM DON_HANG WHERE MaDH=@Code FOR UPDATE";
            using (MySqlCommand select = new(statusQuery, connection, transaction))
            {
                select.Parameters.AddWithValue("@Code", orderCode);
                using MySqlDataReader reader = select.ExecuteReader();
                if (!reader.Read()) throw new InvalidOperationException("Không tìm thấy đơn hàng.");
                oldStatus = reader.GetString("TrangThai");
                oldWarehouse = reader.IsDBNull(reader.GetOrdinal("MaKho")) ? null : reader.GetString("MaKho").Trim();
            }
            if (oldStatus == status) { transaction.Commit(); return; }
            bool validTransition = oldStatus switch
            {
                "Đang xử lý" or "Chờ xác nhận" => status is "Đã xác nhận" or "Đã hủy",
                "Đã xác nhận" => status is "Đang giao hàng" or "Đã hủy",
                "Đang giao hàng" => status is "Đã giao" or "Hoàn thành" or "Đã hủy",
                _ => false
            };
            if (!validTransition) throw new InvalidOperationException("Không thể chuyển sang trạng thái đã chọn từ trạng thái hiện tại.");
            if (oldStatus is "Đã giao" or "Hoàn thành" or "Đã hủy")
                throw new InvalidOperationException("Đơn hàng đã ở trạng thái kết thúc và không thể thay đổi.");
            if (status is "Đã giao" or "Hoàn thành")
            {
                if (oldStatus is "Đã giao" or "Hoàn thành") throw new InvalidOperationException("Đơn hàng đã hoàn tất; không lặp lại thao tác trừ tồn.");
                string selectedWarehouse = oldWarehouse ?? warehouseCode;
                if (string.IsNullOrWhiteSpace(selectedWarehouse)) throw new InvalidOperationException("Cần chọn kho để xuất hàng.");
                using (MySqlCommand lines = new("SELECT MaSP,SoLuongDat FROM CHI_TIET_DON_HANG WHERE MaDH=@Code FOR UPDATE", connection, transaction))
                {
                    lines.Parameters.AddWithValue("@Code", orderCode);
                    using MySqlDataReader reader = lines.ExecuteReader();
                    List<(string Product, int Quantity)> required = [];
                    while (reader.Read()) required.Add((reader.GetString("MaSP"), reader.GetInt32("SoLuongDat")));
                    reader.Close();
                    if (required.Count == 0) throw new InvalidOperationException("Đơn hàng chưa có sản phẩm để xuất kho.");
                    foreach (var line in required.OrderBy(line => line.Product))
                    {
                        if (line.Quantity <= 0) throw new InvalidOperationException("Số lượng sản phẩm trong đơn không hợp lệ.");
                        using MySqlCommand stock = new("UPDATE TON_KHO SET SoLuongTon=SoLuongTon-@Quantity WHERE MaKho=@Warehouse AND MaSP=@Product AND SoLuongTon>=@Quantity", connection, transaction);
                        stock.Parameters.AddWithValue("@Quantity", line.Quantity);
                        stock.Parameters.AddWithValue("@Warehouse", selectedWarehouse);
                        stock.Parameters.AddWithValue("@Product", line.Product);
                        if (stock.ExecuteNonQuery() != 1) throw new InvalidOperationException($"Kho {selectedWarehouse} không đủ tồn cho sản phẩm {line.Product}.");
                    }
                }
                using MySqlCommand update = new("UPDATE DON_HANG SET TrangThai=@Status,MaKho=@Warehouse WHERE MaDH=@Code", connection, transaction);
                update.Parameters.AddWithValue("@Status", status);
                update.Parameters.AddWithValue("@Warehouse", selectedWarehouse);
                update.Parameters.AddWithValue("@Code", orderCode);
                update.ExecuteNonQuery();
            }
            else
            {
                string updateQuery = warehouseAllocationAvailable
                    ? "UPDATE DON_HANG SET TrangThai=@Status,MaKho=COALESCE(MaKho,@Warehouse) WHERE MaDH=@Code"
                    : "UPDATE DON_HANG SET TrangThai=@Status WHERE MaDH=@Code";
                using MySqlCommand update = new(updateQuery, connection, transaction);
                update.Parameters.AddWithValue("@Status", status);
                if (warehouseAllocationAvailable)
                    update.Parameters.AddWithValue("@Warehouse", string.IsNullOrWhiteSpace(warehouseCode) ? DBNull.Value : warehouseCode);
                update.Parameters.AddWithValue("@Code", orderCode);
                update.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch { transaction.Rollback(); throw; }
    }

    public ThongKeBanHangDTO ThongKeBanHang(DateTime from, DateTime to)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "SELECT COUNT(DISTINCT h.MaDH) AS OrderCount,COUNT(h.MaHoaDon) AS InvoiceCount," +
            "COALESCE(SUM(h.TongSoTienThanhToan),0) AS Revenue," +
            "COALESCE((SELECT SUM(ct.SoLuongDat) FROM HOA_DON paid JOIN CHI_TIET_DON_HANG ct ON ct.MaDH=paid.MaDH " +
            "WHERE paid.TrangThai='Đã thanh toán' AND paid.NgayLapHoaDon>=@From AND paid.NgayLapHoaDon<@To),0) AS ProductsSold " +
            "FROM HOA_DON h WHERE h.TrangThai='Đã thanh toán' AND h.NgayLapHoaDon>=@From AND h.NgayLapHoaDon<@To";
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@From", from);
        command.Parameters.AddWithValue("@To", to);
        using MySqlDataReader reader = command.ExecuteReader();
        reader.Read();
        return new ThongKeBanHangDTO(from, to, reader.GetInt32("OrderCount"), reader.GetInt32("InvoiceCount"), reader.GetDecimal("Revenue"), reader.GetInt32("ProductsSold"));
    }
}
