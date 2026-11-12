using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class KhachHangDAO : IKhachHangDAO
{
    private readonly string connectionString;

    public KhachHangDAO(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Chuỗi kết nối không được để trống.", nameof(connectionString));
        this.connectionString = connectionString;
    }

    public IReadOnlyList<KhachHangDTO> DocDanhSach()
    {
        List<KhachHangDTO> customers = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaKH, HoTen, SDT, Email, DiaChi FROM KHACH_HANG ORDER BY MaKH", connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) customers.Add(DocKhachHang(reader));
        return customers;
    }

    public KhachHangDTO? DocTheoMa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("SELECT MaKH, HoTen, SDT, Email, DiaChi FROM KHACH_HANG WHERE MaKH=@MaKH", connection);
        command.Parameters.AddWithValue("@MaKH", code);
        using MySqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? DocKhachHang(reader) : null;
    }

    public KhachHangDTO Them(KhachHangDTO customer)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "INSERT INTO KHACH_HANG (MaKH, HoTen, SDT, DiaChi, Email) VALUES (@MaKH, @HoTen, @SDT, @DiaChi, @Email)";
        using MySqlCommand command = new(query, connection);
        ThemThamSo(command, customer);
        try { command.ExecuteNonQuery(); }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new InvalidOperationException("Mã khách hàng hoặc email đã được sử dụng.", ex);
        }
        return customer;
    }

    public KhachHangDTO Sua(KhachHangDTO customer)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "UPDATE KHACH_HANG SET HoTen=@HoTen, SDT=@SDT, DiaChi=@DiaChi, Email=@Email WHERE MaKH=@MaKH";
        using MySqlCommand command = new(query, connection);
        ThemThamSo(command, customer);
        int affected;
        try { affected = command.ExecuteNonQuery(); }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new InvalidOperationException("Mã khách hàng hoặc email đã được sử dụng.", ex);
        }
        if (affected == 0 && DocTheoMa(customer.Code) is null)
            throw new InvalidOperationException("Không tìm thấy khách hàng.");
        return customer;
    }

    public void Xoa(string code)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using MySqlCommand command = new("DELETE FROM KHACH_HANG WHERE MaKH=@MaKH", connection);
        command.Parameters.AddWithValue("@MaKH", code);
        try
        {
            if (command.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Không tìm thấy khách hàng.");
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            throw new InvalidOperationException("Khách hàng có giỏ hàng hoặc lịch sử đơn hàng; không thể xóa.", ex);
        }
    }

    private static KhachHangDTO DocKhachHang(MySqlDataReader reader) => new(
        reader.GetString("MaKH").Trim(),
        reader.GetString("HoTen"),
        reader.GetString("SDT"),
        reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString("Email"),
        reader.GetString("DiaChi"));

    private static void ThemThamSo(MySqlCommand command, KhachHangDTO customer)
    {
        command.Parameters.AddWithValue("@MaKH", customer.Code);
        command.Parameters.AddWithValue("@HoTen", customer.Name);
        command.Parameters.AddWithValue("@SDT", customer.Phone);
        command.Parameters.AddWithValue("@DiaChi", customer.Address);
        command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(customer.Email) ? DBNull.Value : customer.Email);
    }
}
