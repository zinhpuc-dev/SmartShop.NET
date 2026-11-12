using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class KhoDAO : IKhoDAO
{
    private readonly string connectionString;

    public KhoDAO(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Chuỗi kết nối không được để trống.", nameof(connectionString));
        this.connectionString = connectionString;
    }

    public IReadOnlyList<KhoDTO> DocDanhSach()
    {
        List<KhoDTO> rows = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaKho, TenKho FROM KHO ORDER BY MaKho", connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new KhoDTO { Code = reader.GetString("MaKho").Trim(), Name = reader.GetString("TenKho") });
        return rows;
    }

    public KhoDTO? DocTheoMa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaKho, TenKho FROM KHO WHERE MaKho=@Code", connection);
        command.Parameters.AddWithValue("@Code", code);
        using MySqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? new KhoDTO { Code = reader.GetString("MaKho").Trim(), Name = reader.GetString("TenKho") } : null;
    }

    public void Them(KhoDTO warehouse)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("INSERT INTO KHO (MaKho, TenKho) VALUES (@Code, @Name)", connection);
        command.Parameters.AddWithValue("@Code", warehouse.Code);
        command.Parameters.AddWithValue("@Name", warehouse.Name);
        command.ExecuteNonQuery();
    }

    public void Sua(KhoDTO warehouse)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("UPDATE KHO SET TenKho=@Name WHERE MaKho=@Code", connection);
        command.Parameters.AddWithValue("@Code", warehouse.Code);
        command.Parameters.AddWithValue("@Name", warehouse.Name);
        if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy kho.");
    }

    public void Xoa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("DELETE FROM KHO WHERE MaKho=@Code", connection);
        command.Parameters.AddWithValue("@Code", code);
        try
        {
            if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy kho.");
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            throw new InvalidOperationException("Kho có tồn kho, phiếu nhập hoặc đơn hàng liên kết nên không thể xóa.", ex);
        }
    }

    public IReadOnlyList<PhieuNhapDTO> DocPhieuNhap()
    {
        List<PhieuNhapDTO> rows = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "SELECT pn.MaPN, pn.MaNCC, n.TenNCC, pn.MaKho, k.TenKho, pn.NgayNhap, " +
            "COALESCE(SUM(ct.ThanhTien),0) AS TongTien, COUNT(ct.MaSP) AS SoDong " +
            "FROM PHIEU_NHAP pn JOIN NHA_CUNG_CAP n ON n.MaNCC=pn.MaNCC JOIN KHO k ON k.MaKho=pn.MaKho " +
            "LEFT JOIN CHI_TIET_PHIEU_NHAP ct ON ct.MaPN=pn.MaPN GROUP BY pn.MaPN, pn.MaNCC, n.TenNCC, pn.MaKho, k.TenKho, pn.NgayNhap ORDER BY pn.NgayNhap DESC";
        using MySqlCommand command = new(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new PhieuNhapDTO(reader.GetString("MaPN").Trim(), reader.GetString("MaNCC").Trim(),
            reader.GetString("TenNCC"), reader.GetString("MaKho").Trim(), reader.GetString("TenKho"), reader.GetDateTime("NgayNhap"),
            reader.GetDecimal("TongTien"), reader.GetInt32("SoDong")));
        return rows;
    }

    public void LapPhieuNhap(DuLieuPhieuNhap draft)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlTransaction transaction = connection.BeginTransaction();
        try
        {
            using (MySqlCommand header = new("INSERT INTO PHIEU_NHAP (MaPN, MaNCC, MaKho, NgayNhap) VALUES (@Code, @Supplier, @Warehouse, @Date)", connection, transaction))
            {
                header.Parameters.AddWithValue("@Code", draft.Code);
                header.Parameters.AddWithValue("@Supplier", draft.SupplierCode);
                header.Parameters.AddWithValue("@Warehouse", draft.WarehouseCode);
                header.Parameters.AddWithValue("@Date", DateTime.Now);
                header.ExecuteNonQuery();
            }
            foreach (DuLieuDongNhap line in draft.Lines)
            {
                decimal total = checked(line.UnitCost * line.Quantity);
                using (MySqlCommand detail = new("INSERT INTO CHI_TIET_PHIEU_NHAP (MaPN, MaSP, SoLuongNhap, DonGiaNhap, ThanhTien) VALUES (@Code, @Product, @Quantity, @UnitCost, @Total)", connection, transaction))
                {
                    detail.Parameters.AddWithValue("@Code", draft.Code);
                    detail.Parameters.AddWithValue("@Product", line.ProductCode);
                    detail.Parameters.AddWithValue("@Quantity", line.Quantity);
                    detail.Parameters.AddWithValue("@UnitCost", line.UnitCost);
                    detail.Parameters.AddWithValue("@Total", total);
                    detail.ExecuteNonQuery();
                }
                using MySqlCommand stock = new("INSERT INTO TON_KHO (MaKho, MaSP, SoLuongTon) VALUES (@Warehouse, @Product, @Quantity) ON DUPLICATE KEY UPDATE SoLuongTon=SoLuongTon+VALUES(SoLuongTon)", connection, transaction);
                stock.Parameters.AddWithValue("@Warehouse", draft.WarehouseCode);
                stock.Parameters.AddWithValue("@Product", line.ProductCode);
                stock.Parameters.AddWithValue("@Quantity", line.Quantity);
                stock.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public IReadOnlyList<ChiTietPhieuNhapDTO> DocChiTietPhieuNhap(string code)
    {
        List<ChiTietPhieuNhapDTO> rows = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT ct.MaSP,p.TenSP,ct.SoLuongNhap,ct.DonGiaNhap,ct.ThanhTien FROM CHI_TIET_PHIEU_NHAP ct JOIN SAN_PHAM p ON p.MaSP=ct.MaSP WHERE ct.MaPN=@Code ORDER BY ct.MaSP", connection);
        command.Parameters.AddWithValue("@Code", code);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new ChiTietPhieuNhapDTO(reader.GetString(0).Trim(), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(3), reader.GetDecimal(4)));
        return rows;
    }

    public IReadOnlyList<TonKhoDTO> DocTonKho()
    {
        List<TonKhoDTO> rows = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT t.MaKho,k.TenKho,t.MaSP,p.TenSP,t.SoLuongTon FROM TON_KHO t JOIN KHO k ON k.MaKho=t.MaKho JOIN SAN_PHAM p ON p.MaSP=t.MaSP ORDER BY t.MaKho,t.MaSP", connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new TonKhoDTO(reader.GetString(0).Trim(), reader.GetString(1), reader.GetString(2).Trim(), reader.GetString(3), reader.GetInt32(4)));
        return rows;
    }
}
