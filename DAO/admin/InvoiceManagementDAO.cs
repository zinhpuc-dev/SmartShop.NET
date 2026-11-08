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
        foreach (ManagedInvoice invoice in invoices)
        {
            using MySqlCommand lines = new("SELECT ct.MaSP,p.TenSP,ct.SoLuongDat,ct.DonGia,ct.DonGiaThucTe,ct.ThanhTien FROM CHI_TIET_DON_HANG ct JOIN SAN_PHAM p ON p.MaSP=ct.MaSP WHERE ct.MaDH=@Code ORDER BY ct.MaSP", connection);
            lines.Parameters.AddWithValue("@Code", invoice.OrderCode);
            using MySqlDataReader lineReader = lines.ExecuteReader();
            List<ManagedOrderLine> items = [];
            while (lineReader.Read()) items.Add(new ManagedOrderLine(lineReader.GetString("MaSP").Trim(), lineReader.GetString("TenSP"),
                lineReader.GetInt32("SoLuongDat"), lineReader.GetDecimal("DonGia"), lineReader.GetDecimal("DonGiaThucTe"), lineReader.GetDecimal("ThanhTien")));
            int index = invoices.IndexOf(invoice);
            invoices[index] = invoice with { Lines = items };
        }
        return invoices;
    }
}
