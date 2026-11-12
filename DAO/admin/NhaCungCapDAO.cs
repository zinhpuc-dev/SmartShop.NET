using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class NhaCungCapDAO : INhaCungCapDAO
{
    private readonly string connectionString;

    public NhaCungCapDAO(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Chuỗi kết nối không được để trống.", nameof(connectionString));
        this.connectionString = connectionString;
    }

    public IReadOnlyList<NhaCungCapDTO> DocDanhSach()
    {
        List<NhaCungCapDTO> suppliers = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaNCC, TenNCC, DiaChi, Email, SDT FROM NHA_CUNG_CAP ORDER BY MaNCC", connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) suppliers.Add(DocNhaCungCap(reader));
        return suppliers;
    }

    public NhaCungCapDTO? DocTheoMa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaNCC, TenNCC, DiaChi, Email, SDT FROM NHA_CUNG_CAP WHERE MaNCC=@MaNCC", connection);
        command.Parameters.AddWithValue("@MaNCC", code);
        using MySqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? DocNhaCungCap(reader) : null;
    }

    public NhaCungCapDTO Them(NhaCungCapDTO supplier)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "INSERT INTO NHA_CUNG_CAP (MaNCC, TenNCC, DiaChi, Email, SDT) VALUES (@MaNCC, @TenNCC, @DiaChi, @Email, @SDT)";
        using MySqlCommand command = new(query, connection);
        ThemThamSo(command, supplier);
        try { command.ExecuteNonQuery(); }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new InvalidOperationException("Mã nhà cung cấp đã được sử dụng.", ex);
        }
        return supplier;
    }

    public NhaCungCapDTO Sua(NhaCungCapDTO supplier)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "UPDATE NHA_CUNG_CAP SET TenNCC=@TenNCC, DiaChi=@DiaChi, Email=@Email, SDT=@SDT WHERE MaNCC=@MaNCC";
        using MySqlCommand command = new(query, connection);
        ThemThamSo(command, supplier);
        int affected = command.ExecuteNonQuery();
        if (affected == 0 && DocTheoMa(supplier.Code) is null)
            throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");
        return supplier;
    }

    public void Xoa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("DELETE FROM NHA_CUNG_CAP WHERE MaNCC=@MaNCC", connection);
        command.Parameters.AddWithValue("@MaNCC", code);
        try
        {
            if (command.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            throw new InvalidOperationException("Nhà cung cấp có phiếu nhập liên kết; không thể xóa dữ liệu lịch sử.", ex);
        }
    }

    private static NhaCungCapDTO DocNhaCungCap(MySqlDataReader reader) => new(
        reader.GetString("MaNCC").Trim(),
        reader.GetString("TenNCC"),
        reader.IsDBNull(reader.GetOrdinal("SDT")) ? string.Empty : reader.GetString("SDT"),
        reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString("Email"),
        reader.IsDBNull(reader.GetOrdinal("DiaChi")) ? string.Empty : reader.GetString("DiaChi"));

    private static void ThemThamSo(MySqlCommand command, NhaCungCapDTO supplier)
    {
        command.Parameters.AddWithValue("@MaNCC", supplier.Code);
        command.Parameters.AddWithValue("@TenNCC", supplier.Name);
        command.Parameters.AddWithValue("@DiaChi", string.IsNullOrWhiteSpace(supplier.Address) ? DBNull.Value : supplier.Address);
        command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(supplier.Email) ? DBNull.Value : supplier.Email);
        command.Parameters.AddWithValue("@SDT", string.IsNullOrWhiteSpace(supplier.Phone) ? DBNull.Value : supplier.Phone);
    }
}
