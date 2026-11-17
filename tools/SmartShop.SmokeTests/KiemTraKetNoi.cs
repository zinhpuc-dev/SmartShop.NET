using System.Data;
using MySql.Data.MySqlClient;
using SmartShop.DAO;

internal static class KiemTraKetNoi
{
    public static void Chay()
    {
        static void KiemTra(bool dung, string moTa)
        {
            if (!dung) throw new Exception(moTa);
            Console.WriteLine("PASS CONNECTION " + moTa);
        }

        foreach (var chuoi in new[] { "", "   " })
        {
            try { _ = new MyConnection(chuoi); }
            catch (ArgumentException)
            {
                KiemTra(true, "Từ chối chuỗi kết nối rỗng trước khi mở CSDL");
                continue;
            }
            throw new Exception("Chuỗi kết nối rỗng chưa bị từ chối.");
        }

        var nguon = new MyConnection("Server=localhost;Database=kiem_tra_khong_mo;");
        using var thuNhat = nguon.TaoKetNoi();
        using var thuHai = nguon.TaoKetNoi();
        KiemTra(!ReferenceEquals(thuNhat, thuHai) && thuNhat.State == ConnectionState.Closed
            && thuHai.State == ConnectionState.Closed, "Mỗi thao tác nhận kết nối riêng, không chia sẻ kết nối đang mở");
        var cauHinh = new MySqlConnectionStringBuilder(thuNhat.ConnectionString);
        KiemTra(cauHinh.CharacterSet == "utf8mb4" && cauHinh.Database == "kiem_tra_khong_mo",
            "Kết nối mặc định hỗ trợ tiếng Việt và giữ database cấu hình");
        using var chiDinh = new MyConnection("Server=localhost;Character Set=utf8;").TaoKetNoi();
        KiemTra(new MySqlConnectionStringBuilder(chiDinh.ConnectionString).CharacterSet == "utf8",
            "Giữ nguyên bộ mã khi cấu hình đã chỉ định");
    }
}
