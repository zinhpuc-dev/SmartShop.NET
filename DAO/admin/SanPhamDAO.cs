using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO
{
    public class SanPhamDAO : ISanPhamDAO
    {
        private readonly string? connectionString;

        public SanPhamDAO()
        {
        }

        public SanPhamDAO(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Chuỗi kết nối không được để trống.", nameof(connectionString));
            this.connectionString = connectionString;
        }

        public List<SanPhamQuanTriDTO> DocDanhSach()
        {
            List<SanPhamQuanTriDTO> product = new List<SanPhamQuanTriDTO>();
            using (MySqlConnection connection = TaoKetNoi())
            {
                connection.Open();
                string query = "SELECT p.MaSP, p.MaLoai, p.TenSP, p.MoTa, p.DonViTinh, p.GiaNiemYet, p.GiaBan, p.TrangThai, " +
                    "CAST(COALESCE(SUM(t.SoLuongTon), 0) AS SIGNED) AS TongSoLuongTon " +
                    "FROM SAN_PHAM p LEFT JOIN TON_KHO t ON t.MaSP=p.MaSP " +
                    "GROUP BY p.MaSP, p.MaLoai, p.TenSP, p.MoTa, p.DonViTinh, p.GiaNiemYet, p.GiaBan, p.TrangThai ORDER BY p.MaSP";
                using MySqlCommand command = new MySqlCommand(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string maSP = reader.GetString("MaSP");
                    string maLoai = reader.GetString("MaLoai");
                    string tenSP = reader.GetString("TenSP");
                    string moTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa");
                    string donViTinh = reader.GetString("DonViTinh");
                    decimal giaNiemYet = reader.GetDecimal("GiaNiemYet");
                    decimal giaBan = reader.GetDecimal("GiaBan");
                    string trangThai = reader.GetBoolean("TrangThai") ? "1" : "0";
                    SanPhamQuanTriDTO pm = new SanPhamQuanTriDTO(maSP, maLoai, tenSP, moTa, donViTinh, giaNiemYet, giaBan, trangThai);
                    pm.TongSoLuongTon = reader.GetInt32("TongSoLuongTon");
                    product.Add(pm);
                }
            }
            return product;
        }

        public List<SanPhamQuanTriDTO> TimKiem(string keyword)
        {
            List<SanPhamQuanTriDTO> product = new List<SanPhamQuanTriDTO>();
            using (MySqlConnection connection = TaoKetNoi())
            {
                connection.Open();
                string query = "SELECT p.MaSP, p.MaLoai, p.TenSP, p.MoTa, p.DonViTinh, p.GiaNiemYet, p.GiaBan, p.TrangThai, " +
                    "CAST(COALESCE(SUM(t.SoLuongTon), 0) AS SIGNED) AS TongSoLuongTon " +
                    "FROM SAN_PHAM p LEFT JOIN TON_KHO t ON t.MaSP=p.MaSP " +
                    "WHERE p.MaSP LIKE @TuKhoa OR p.TenSP LIKE @TuKhoa " +
                    "GROUP BY p.MaSP, p.MaLoai, p.TenSP, p.MoTa, p.DonViTinh, p.GiaNiemYet, p.GiaBan, p.TrangThai ORDER BY p.MaSP";
                using MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@TuKhoa", $"%{keyword?.Trim() ?? string.Empty}%");
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SanPhamQuanTriDTO item = new SanPhamQuanTriDTO(
                        reader.GetString("MaSP"), reader.GetString("MaLoai"), reader.GetString("TenSP"),
                        reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa"),
                        reader.GetString("DonViTinh"), reader.GetDecimal("GiaNiemYet"), reader.GetDecimal("GiaBan"),
                        reader.GetBoolean("TrangThai") ? "1" : "0");
                    item.TongSoLuongTon = reader.GetInt32("TongSoLuongTon");
                    product.Add(item);
                }
            }
            return product;
        }

        public SanPhamQuanTriDTO? DocTheoMa(string maSP)
        {
            using (MySqlConnection connection = TaoKetNoi())
            {
                connection.Open();
                const string query = "SELECT MaSP, MaLoai, TenSP, MoTa, DonViTinh, GiaNiemYet, GiaBan, TrangThai FROM SAN_PHAM WHERE MaSP=@MaSP";
                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@MaSP", maSP);
                using MySqlDataReader reader = command.ExecuteReader();
                if (!reader.Read()) return null;
                return new SanPhamQuanTriDTO(reader.GetString("MaSP"), reader.GetString("MaLoai"), reader.GetString("TenSP"),
                    reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa"),
                    reader.GetString("DonViTinh"), reader.GetDecimal("GiaNiemYet"), reader.GetDecimal("GiaBan"),
                    reader.GetBoolean("TrangThai") ? "1" : "0");
            }
        }

        public List<LoaiSanPhamDTO> DocLoaiSanPham()
        {
            List<LoaiSanPhamDTO> categories = [];
            using MySqlConnection connection = TaoKetNoi();
            connection.Open();
            using MySqlCommand command = new("SELECT MaLoai, TrangThai FROM LOAI_SAN_PHAM ORDER BY MaLoai", connection);
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) categories.Add(new LoaiSanPhamDTO { MaLoai = reader.GetString("MaLoai"), TrangThai = reader.GetBoolean("TrangThai") });
            return categories;
        }

        public bool KiemTraLoaiSanPham(string maLoai)
        {
            using MySqlConnection connection = TaoKetNoi();
            connection.Open();
            using MySqlCommand command = new("SELECT COUNT(*) FROM LOAI_SAN_PHAM WHERE MaLoai=@MaLoai", connection);
            command.Parameters.AddWithValue("@MaLoai", maLoai);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public void ThemLoaiSanPham(LoaiSanPhamDTO category)
        {
            using MySqlConnection connection = TaoKetNoi();
            connection.Open();
            using MySqlCommand command = new("INSERT INTO LOAI_SAN_PHAM (MaLoai, TrangThai) VALUES (@MaLoai, @TrangThai)", connection);
            command.Parameters.AddWithValue("@MaLoai", category.MaLoai);
            command.Parameters.AddWithValue("@TrangThai", category.TrangThai);
            command.ExecuteNonQuery();
        }

        public void SuaLoaiSanPham(LoaiSanPhamDTO category)
        {
            using MySqlConnection connection = TaoKetNoi();
            connection.Open();
            using MySqlCommand command = new("UPDATE LOAI_SAN_PHAM SET TrangThai=@TrangThai WHERE MaLoai=@MaLoai", connection);
            command.Parameters.AddWithValue("@MaLoai", category.MaLoai);
            command.Parameters.AddWithValue("@TrangThai", category.TrangThai);
            if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy loại sản phẩm.");
        }

        public void XoaLoaiSanPham(string maLoai)
        {
            using MySqlConnection connection = TaoKetNoi();
            connection.Open();
            using MySqlCommand command = new("DELETE FROM LOAI_SAN_PHAM WHERE MaLoai=@MaLoai", connection);
            command.Parameters.AddWithValue("@MaLoai", maLoai);
            if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy loại sản phẩm.");
        }

        public List<TonKhoSanPhamDTO> DocTonKhoTheoSanPham(string maSP)
        {
            List<TonKhoSanPhamDTO> stock = [];
            using MySqlConnection connection = TaoKetNoi();
            connection.Open();
            const string query = "SELECT k.MaKho, k.TenKho, COALESCE(t.SoLuongTon, 0) AS SoLuongTon FROM KHO k " +
                "LEFT JOIN TON_KHO t ON t.MaKho=k.MaKho AND t.MaSP=@MaSP ORDER BY k.MaKho";
            using MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@MaSP", maSP);
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) stock.Add(new TonKhoSanPhamDTO(reader.GetString("MaKho"), reader.GetString("TenKho"), reader.GetInt32("SoLuongTon")));
            return stock;
        }

        public void Sua(SanPhamQuanTriDTO pm)
        {
            using (MySqlConnection connection = TaoKetNoi())
            {
                connection.Open();
                string query = "UPDATE SAN_PHAM SET MaLoai=@MaLoai, TenSP=@TenSP, MoTa=@MoTa, " +
                    "DonViTinh=@DonViTinh, GiaNiemYet=@GiaNiemYet, GiaBan=@GiaBan, TrangThai=@TrangThai WHERE MaSP=@MaSP";
                using MySqlCommand command = new MySqlCommand(query, connection);
                ThemThamSoSanPham(command, pm);
                command.ExecuteNonQuery();
            }
        }

        public void Them(SanPhamQuanTriDTO pm)
        {
            using (MySqlConnection connection = TaoKetNoi())
            {
                connection.Open();
                string query = "INSERT INTO SAN_PHAM " +
                    "(MaSP, MaLoai, TenSP, MoTa, DonViTinh, GiaNiemYet, GiaBan, TrangThai) " +
                    "VALUES (@MaSP, @MaLoai, @TenSP, @MoTa, @DonViTinh, @GiaNiemYet, @GiaBan, @TrangThai)";
                using MySqlCommand command = new MySqlCommand(query, connection);
                ThemThamSoSanPham(command, pm);
                command.ExecuteNonQuery();
            }
        }

        public bool KiemTraSanPham(string ma)
        {
            using (MySqlConnection connection = TaoKetNoi())
            {
                connection.Open();
                using MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM SAN_PHAM WHERE MaSP=@MaSP", connection);
                command.Parameters.AddWithValue("@MaSP", ma);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public void DoiTrangThai(string maSP, bool active)
        {
            using MySqlConnection connection = TaoKetNoi();
            connection.Open();
            using MySqlCommand command = new("UPDATE SAN_PHAM SET TrangThai=@TrangThai WHERE MaSP=@MaSP", connection);
            command.Parameters.AddWithValue("@TrangThai", active);
            command.Parameters.AddWithValue("@MaSP", maSP);
            command.ExecuteNonQuery();
        }

        public void Xoa(string maSP)
        {
            using (MySqlConnection connection = TaoKetNoi())
            {
                connection.Open();
                using MySqlCommand command = new MySqlCommand("DELETE FROM SAN_PHAM WHERE MaSP=@MaSP", connection);
                command.Parameters.AddWithValue("@MaSP", maSP);
                try
                {
                    if (command.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException("Không tìm thấy sản phẩm.");
                }
                catch (MySqlException ex) when (ex.Number == 1451)
                {
                    throw new InvalidOperationException("Sản phẩm đang liên kết với dữ liệu khác nên không thể xóa. Bạn có thể chọn Ngừng bán.", ex);
                }
            }
        }

        private static void ThemThamSoSanPham(MySqlCommand command, SanPhamQuanTriDTO pm)
        {
            command.Parameters.AddWithValue("@MaSP", pm.MaSP);
            command.Parameters.AddWithValue("@MaLoai", pm.MaLoai);
            command.Parameters.AddWithValue("@TenSP", pm.TenSP);
            command.Parameters.AddWithValue("@MoTa", string.IsNullOrWhiteSpace(pm.MoTa) ? DBNull.Value : pm.MoTa);
            command.Parameters.AddWithValue("@DonViTinh", pm.DonViTinh);
            command.Parameters.AddWithValue("@GiaNiemYet", pm.GiaNiemYet);
            command.Parameters.AddWithValue("@GiaBan", pm.GiaBan);
            command.Parameters.AddWithValue("@TrangThai", pm.TrangThai == "1");
        }

        private MySqlConnection TaoKetNoi()
        {
            string? configuredConnection = connectionString ?? Environment.GetEnvironmentVariable("SMARTSHOP_DB_CONNECTION");
            if (string.IsNullOrWhiteSpace(configuredConnection))
                throw new InvalidOperationException("Vui lòng cấu hình biến môi trường SMARTSHOP_DB_CONNECTION.");
            return new MySqlConnection(configuredConnection);
        }
    }
}