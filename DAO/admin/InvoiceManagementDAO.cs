using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class InvoiceManagementDAO(string connectionString) : IInvoiceManagementRepository
{
    public IReadOnlyList<ManagedInvoice> GetInvoices()
    {
        List<ManagedInvoice> invoices = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "SELECT h.MaHoaDon,h.MaDH,h.MaNV,n.TenNV,h.NgayLapHoaDon,h.TongSoTienThanhToan,h.PhuongThucThanhToan,h.TrangThai " +
            "FROM HOA_DON h JOIN NHAN_VIEN n ON n.MaNV=h.MaNV ORDER BY h.NgayLapHoaDon DESC,h.MaHoaDon";
        using (MySqlCommand command = new(query, connection))
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read()) invoices.Add(new ManagedInvoice(reader.GetString("MaHoaDon").Trim(), reader.GetString("MaDH").Trim(),
                reader.GetString("MaNV").Trim(), reader.GetString("TenNV"), reader.GetDateTime("NgayLapHoaDon"), reader.GetDecimal("TongSoTienThanhToan"),
                reader.GetString("PhuongThucThanhToan"), reader.GetString("TrangThai"), []));
        }
        for (int index = 0; index < invoices.Count; index++)
        {
            ManagedInvoice invoice = invoices[index];
            using MySqlCommand lines = new("SELECT ct.MaSP,p.TenSP,ct.SoLuongDat,ct.DonGia,ct.DonGiaThucTe,ct.ThanhTien FROM CHI_TIET_DON_HANG ct JOIN SAN_PHAM p ON p.MaSP=ct.MaSP WHERE ct.MaDH=@Code ORDER BY ct.MaSP", connection);
            lines.Parameters.AddWithValue("@Code", invoice.OrderCode);
            using MySqlDataReader lineReader = lines.ExecuteReader();
            List<ManagedOrderLine> items = [];
            while (lineReader.Read()) items.Add(new ManagedOrderLine(lineReader.GetString("MaSP").Trim(), lineReader.GetString("TenSP"),
                lineReader.GetInt32("SoLuongDat"), lineReader.GetDecimal("DonGia"), lineReader.GetDecimal("DonGiaThucTe"), lineReader.GetDecimal("ThanhTien")));
            invoices[index] = invoice with { Lines = items };
        }
        return invoices;
    }

    public void CreateInvoice(string orderCode, string employeeCode, string paymentMethod)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlTransaction transaction = connection.BeginTransaction();
        try
        {
            decimal total;
            using (MySqlCommand order = new("SELECT TongTien,TrangThai FROM DON_HANG WHERE MaDH=@Order FOR UPDATE", connection, transaction))
            {
                order.Parameters.AddWithValue("@Order", orderCode);
                using MySqlDataReader reader = order.ExecuteReader();
                if (!reader.Read()) throw new InvalidOperationException("Không tìm thấy đơn hàng.");
                if (reader.GetString("TrangThai") is not ("Đã giao" or "Hoàn thành"))
                    throw new InvalidOperationException("Chỉ lập hóa đơn cho đơn hàng đã giao hoặc hoàn thành.");
                total = reader.GetDecimal("TongTien");
            }
            using (MySqlCommand existing = new("SELECT COUNT(*) FROM HOA_DON WHERE MaDH=@Order", connection, transaction))
            {
                existing.Parameters.AddWithValue("@Order", orderCode);
                if (Convert.ToInt32(existing.ExecuteScalar()) > 0)
                    throw new InvalidOperationException("Đơn hàng đã có hóa đơn.");
            }
            using (MySqlCommand detail = new("SELECT COUNT(*),COALESCE(SUM(ThanhTien),0) FROM CHI_TIET_DON_HANG WHERE MaDH=@Order", connection, transaction))
            {
                detail.Parameters.AddWithValue("@Order", orderCode);
                using MySqlDataReader reader = detail.ExecuteReader();
                reader.Read();
                if (reader.GetInt32(0) == 0 || total <= 0 || total != reader.GetDecimal(1))
                    throw new InvalidOperationException("Tổng tiền đơn hàng không khớp chi tiết; cần đối chiếu trước khi lập hóa đơn.");
            }
            using MySqlCommand insert = new("INSERT INTO HOA_DON (MaHoaDon,MaDH,MaNV,NgayLapHoaDon,TongSoTienThanhToan,PhuongThucThanhToan,TrangThai) " +
                "VALUES (@Code,@Order,@Employee,@Date,@Total,@Method,'Chờ thanh toán')", connection, transaction);
            insert.Parameters.AddWithValue("@Code", "HD" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant());
            insert.Parameters.AddWithValue("@Order", orderCode);
            insert.Parameters.AddWithValue("@Employee", employeeCode);
            insert.Parameters.AddWithValue("@Date", DateTime.Now);
            insert.Parameters.AddWithValue("@Total", total);
            insert.Parameters.AddWithValue("@Method", paymentMethod);
            insert.ExecuteNonQuery();
            transaction.Commit();
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Hóa đơn đã tồn tại hoặc mã bị trùng. Vui lòng tải lại và thử lại.", ex);
        }
        catch { transaction.Rollback(); throw; }
    }

    public void MarkPaid(string invoiceCode)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlTransaction transaction = connection.BeginTransaction();
        try
        {
            using MySqlCommand select = new("SELECT TrangThai FROM HOA_DON WHERE MaHoaDon=@Code FOR UPDATE", connection, transaction);
            select.Parameters.AddWithValue("@Code", invoiceCode);
            string? status = select.ExecuteScalar() as string;
            if (status is null) throw new InvalidOperationException("Không tìm thấy hóa đơn.");
            if (status == "Đã thanh toán") { transaction.Commit(); return; }
            if (status != "Chờ thanh toán") throw new InvalidOperationException("Hóa đơn không ở trạng thái chờ thanh toán.");
            using MySqlCommand update = new("UPDATE HOA_DON SET TrangThai='Đã thanh toán' WHERE MaHoaDon=@Code", connection, transaction);
            update.Parameters.AddWithValue("@Code", invoiceCode);
            update.ExecuteNonQuery();
            transaction.Commit();
        }
        catch { transaction.Rollback(); throw; }
    }
}
