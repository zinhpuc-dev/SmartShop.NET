using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class NhanVienDAO : INhanVienDAO
{
    private readonly string connectionString;

    public NhanVienDAO(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Chuỗi kết nối không được để trống.", nameof(connectionString));
        this.connectionString = connectionString;
    }

    public IReadOnlyList<NhanVienDTO> DocDanhSach()
    {
        List<NhanVienDTO> employees = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaNV, TenNV, ChucVu, SDT, Email FROM NHAN_VIEN ORDER BY MaNV", connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) employees.Add(DocNhanVien(reader));
        return employees;
    }

    public NhanVienDTO? DocTheoMa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaNV, TenNV, ChucVu, SDT, Email FROM NHAN_VIEN WHERE MaNV=@MaNV", connection);
        command.Parameters.AddWithValue("@MaNV", code);
        using MySqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? DocNhanVien(reader) : null;
    }

    public NhanVienDTO Them(NhanVienDTO employee)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "INSERT INTO NHAN_VIEN (MaNV, TenNV, ChucVu, SDT, Email) VALUES (@MaNV, @TenNV, @ChucVu, @SDT, @Email)";
        using MySqlCommand command = new(query, connection);
        ThemThamSo(command, employee);
        try { command.ExecuteNonQuery(); }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new InvalidOperationException("Mã nhân viên hoặc email đã được sử dụng.", ex);
        }
        return employee;
    }

    public NhanVienDTO Sua(NhanVienDTO employee)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "UPDATE NHAN_VIEN SET TenNV=@TenNV, ChucVu=@ChucVu, SDT=@SDT, Email=@Email WHERE MaNV=@MaNV";
        using MySqlCommand command = new(query, connection);
        ThemThamSo(command, employee);
        int affected;
        try { affected = command.ExecuteNonQuery(); }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new InvalidOperationException("Mã nhân viên hoặc email đã được sử dụng.", ex);
        }
        if (affected == 0 && DocTheoMa(employee.Code) is null)
            throw new InvalidOperationException("Không tìm thấy nhân viên.");
        return employee;
    }

    public void Xoa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("DELETE FROM NHAN_VIEN WHERE MaNV=@MaNV", connection);
        command.Parameters.AddWithValue("@MaNV", code);
        try
        {
            if (command.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Không tìm thấy nhân viên.");
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            throw new InvalidOperationException("Nhân viên có hóa đơn liên kết; không thể xóa hồ sơ lịch sử.", ex);
        }
    }

    private static NhanVienDTO DocNhanVien(MySqlDataReader reader) => new(
        reader.GetString("MaNV").Trim(),
        reader.GetString("TenNV"),
        reader.GetString("ChucVu"),
        reader.IsDBNull(reader.GetOrdinal("SDT")) ? string.Empty : reader.GetString("SDT"),
        reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString("Email"));

    private static void ThemThamSo(MySqlCommand command, NhanVienDTO employee)
    {
        command.Parameters.AddWithValue("@MaNV", employee.Code);
        command.Parameters.AddWithValue("@TenNV", employee.Name);
        command.Parameters.AddWithValue("@ChucVu", employee.Position);
        command.Parameters.AddWithValue("@SDT", string.IsNullOrWhiteSpace(employee.Phone) ? DBNull.Value : employee.Phone);
        command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(employee.Email) ? DBNull.Value : employee.Email);
    }
}
