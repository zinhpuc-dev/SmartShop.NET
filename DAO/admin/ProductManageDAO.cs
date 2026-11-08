using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO
{
    public class ProductManageDAO : IProductManage
    {
        private readonly string? connectionString;

        public ProductManageDAO()
        {
        }

        public ProductManageDAO(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Chuỗi kết nối không được để trống.", nameof(connectionString));
            this.connectionString = connectionString;
        }

        public List<ProductManage> ReadData()
        {
            List<ProductManage> product = new List<ProductManage>();
            using (MySqlConnection connection = CreateConnection())
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
                    ProductManage pm = new ProductManage(maSP, maLoai, tenSP, moTa, donViTinh, giaNiemYet, giaBan, trangThai);
                    pm.TongSoLuongTon = reader.GetInt32("TongSoLuongTon");
                    product.Add(pm);
                }
            }
            return product;
        }

        public List<ProductManage> Search(string keyword)
        {
            List<ProductManage> product = new List<ProductManage>();
            using (MySqlConnection connection = CreateConnection())
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
                    ProductManage item = new ProductManage(
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

        public ProductManage? GetByCode(string maSP)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                connection.Open();
                const string query = "SELECT MaSP, MaLoai, TenSP, MoTa, DonViTinh, GiaNiemYet, GiaBan, TrangThai FROM SAN_PHAM WHERE MaSP=@MaSP";
                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@MaSP", maSP);
                using MySqlDataReader reader = command.ExecuteReader();
                if (!reader.Read()) return null;
                return new ProductManage(reader.GetString("MaSP"), reader.GetString("MaLoai"), reader.GetString("TenSP"),
                    reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa"),
                    reader.GetString("DonViTinh"), reader.GetDecimal("GiaNiemYet"), reader.GetDecimal("GiaBan"),
                    reader.GetBoolean("TrangThai") ? "1" : "0");
            }
        }

        public List<ProductCategory> ReadCategories()
        {
            List<ProductCategory> categories = [];
            using MySqlConnection connection = CreateConnection();
            connection.Open();
            using MySqlCommand command = new("SELECT MaLoai, TrangThai FROM LOAI_SAN_PHAM ORDER BY MaLoai", connection);
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) categories.Add(new ProductCategory { MaLoai = reader.GetString("MaLoai"), TrangThai = reader.GetBoolean("TrangThai") });
            return categories;
        }

        public bool CheckCategory(string maLoai)
        {
            using MySqlConnection connection = CreateConnection();
            connection.Open();
            using MySqlCommand command = new("SELECT COUNT(*) FROM LOAI_SAN_PHAM WHERE MaLoai=@MaLoai", connection);
            command.Parameters.AddWithValue("@MaLoai", maLoai);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public void InsertCategory(ProductCategory category)
        {
            using MySqlConnection connection = CreateConnection();
            connection.Open();
            using MySqlCommand command = new("INSERT INTO LOAI_SAN_PHAM (MaLoai, TrangThai) VALUES (@MaLoai, @TrangThai)", connection);
            command.Parameters.AddWithValue("@MaLoai", category.MaLoai);
            command.Parameters.AddWithValue("@TrangThai", category.TrangThai);
            command.ExecuteNonQuery();
        }

        public void UpdateCategory(ProductCategory category)
        {
            using MySqlConnection connection = CreateConnection();
            connection.Open();
            using MySqlCommand command = new("UPDATE LOAI_SAN_PHAM SET TrangThai=@TrangThai WHERE MaLoai=@MaLoai", connection);
            command.Parameters.AddWithValue("@MaLoai", category.MaLoai);
            command.Parameters.AddWithValue("@TrangThai", category.TrangThai);
            if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy loại sản phẩm.");
        }

        public void DeleteCategory(string maLoai)
        {
            using MySqlConnection connection = CreateConnection();
            connection.Open();
            using MySqlCommand command = new("DELETE FROM LOAI_SAN_PHAM WHERE MaLoai=@MaLoai", connection);
            command.Parameters.AddWithValue("@MaLoai", maLoai);
            if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy loại sản phẩm.");
        }

        public List<ProductStock> ReadStockByProduct(string maSP)
        {
            List<ProductStock> stock = [];
            using MySqlConnection connection = CreateConnection();
            connection.Open();
            const string query = "SELECT k.MaKho, k.TenKho, COALESCE(t.SoLuongTon, 0) AS SoLuongTon FROM KHO k " +
                "LEFT JOIN TON_KHO t ON t.MaKho=k.MaKho AND t.MaSP=@MaSP ORDER BY k.MaKho";
            using MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@MaSP", maSP);
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) stock.Add(new ProductStock(reader.GetString("MaKho"), reader.GetString("TenKho"), reader.GetInt32("SoLuongTon")));
            return stock;
        }

        public void Update(ProductManage pm)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                connection.Open();
                string query = "UPDATE SAN_PHAM SET MaLoai=@MaLoai, TenSP=@TenSP, MoTa=@MoTa, " +
                    "DonViTinh=@DonViTinh, GiaNiemYet=@GiaNiemYet, GiaBan=@GiaBan, TrangThai=@TrangThai WHERE MaSP=@MaSP";
                using MySqlCommand command = new MySqlCommand(query, connection);
                AddProductParameters(command, pm);
                command.ExecuteNonQuery();
            }
        }

        public void Insert(ProductManage pm)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                connection.Open();
                string query = "INSERT INTO SAN_PHAM " +
                    "(MaSP, MaLoai, TenSP, MoTa, DonViTinh, GiaNiemYet, GiaBan, TrangThai) " +
                    "VALUES (@MaSP, @MaLoai, @TenSP, @MoTa, @DonViTinh, @GiaNiemYet, @GiaBan, @TrangThai)";
                using MySqlCommand command = new MySqlCommand(query, connection);
                AddProductParameters(command, pm);
                command.ExecuteNonQuery();
            }
        }

        public bool CheckProduct(string ma)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                connection.Open();
                using MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM SAN_PHAM WHERE MaSP=@MaSP", connection);
                command.Parameters.AddWithValue("@MaSP", ma);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public void Delete(string maSP)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                connection.Open();
                using MySqlCommand command = new MySqlCommand("DELETE FROM SAN_PHAM WHERE MaSP=@MaSP", connection);
                command.Parameters.AddWithValue("@MaSP", maSP);
                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("Không tìm thấy sản phẩm hoặc sản phẩm đang được tham chiếu.");
            }
        }

        private static void AddProductParameters(MySqlCommand command, ProductManage pm)
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

        private MySqlConnection CreateConnection()
        {
            string? configuredConnection = connectionString ?? Environment.GetEnvironmentVariable("SMARTSHOP_DB_CONNECTION");
            if (string.IsNullOrWhiteSpace(configuredConnection))
                throw new InvalidOperationException("Vui lòng cấu hình biến môi trường SMARTSHOP_DB_CONNECTION.");
            return new MySqlConnection(configuredConnection);
        }
    }
}