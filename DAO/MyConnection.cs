using MySql.Data.MySqlClient;

namespace SmartShop.DAO;

// Lớp kết nối dùng chung theo mẫu 3 lớp; không giữ kết nối mở giữa các thao tác.
public sealed class MyConnection
{
    private readonly string chuoiKetNoi;

    public MyConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Chuỗi kết nối không được để trống.", nameof(connectionString));
        var cauHinh = new MySqlConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(cauHinh.CharacterSet)) cauHinh.CharacterSet = "utf8mb4";
        chuoiKetNoi = cauHinh.ConnectionString;
    }

    public MySqlConnection TaoKetNoi() => new(chuoiKetNoi);

    public MySqlConnection MoKetNoi()
    {
        var ketNoi = TaoKetNoi();
        try
        {
            ketNoi.Open();
            return ketNoi;
        }
        catch
        {
            ketNoi.Dispose();
            throw;
        }
    }
}
