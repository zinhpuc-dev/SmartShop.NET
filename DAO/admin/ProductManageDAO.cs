using MySql.Data.MySqlClient;
using SmartShop.DTO;

namespace SmartShop.DAO
{
    public class ProductManageDAO
    {
        string user = "root";
        string password = "";
        string connString => "Server=localhost;Port=3306;Database=QuanLyCuaHangDienTu_MySQL;Uid=root;Pwd='';";

        MySqlConnection connection=new MySqlConnection();

        public ProductManageDAO()
        {
            if (connection==null)
            {
                try
                {
                    connection = new MySqlConnection(connString);
                    connection.Open();
                    Console.WriteLine("Connection successfull");
                }
                catch
                {
                    Console.WriteLine("Error connection");
                }
            }
        }
        
        public List<ProductManage> ReadData()
        {
            List<ProductManage> product = new List<ProductManage>();
            try
            {
                string query= "SELECT MaSP, MaLoai, TenSP, MoTa, DonViTinh, GiaNiemYet, GiaBan, TrangThai FROM SAN_PHAM";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader=command.ExecuteReader();
                while (reader.Read())
                {
                    string maSP = reader.GetString("MaSP");
                    string maLoai = reader.GetString("MaLoai");
                    string tenSP = reader.GetString("TenSP");
                    string moTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa");
                    string donViTinh = reader.GetString("DonViTinh");
                    int giaNiemYet = reader.GetInt32("GiaNiemYet");
                    int giaBan = reader.GetInt32("GiaBan");
                    string trangThai = reader.GetString("TrangThai");

                    ProductManage pm = new ProductManage(maSP, maLoai, tenSP, moTa, donViTinh, giaNiemYet, giaBan, trangThai);
                    product.Add(pm);
                }

            }
            catch
            {
                Console.WriteLine("Lỗi đọc dữ liệu sản phẩm");
            }
            return product;
        }

        public void Update(ProductManage pm)
        {
            try
            {
                string query = "UPDATE SAN_PHAM SET MaLoai='"+pm.MaLoai+"', TenSP='"+pm.TenSP+"', MoTa='"+pm.MoTa+"', DonViTinh='"+pm.DonViTinh+ "', GiaNiemYet='"+pm.GiaNiemYet+ "', GiaBan='"+pm.GiaBan+ "', TrangThai='"+pm.TrangThai+"' WHERE MaSP='" + pm.MaSP+"'";
                MySqlCommand command = new MySqlCommand(query, connection);
                int r = command.ExecuteNonQuery();
                if (r > 0)
                {
                    Console.WriteLine("Sửa dữ liệu sản phẩm thành công");
                }
                else
                {
                    Console.WriteLine("Lỗi sửa dữ liệu sản phẩm");
                }
            }
            catch
            {
                Console.WriteLine("Lỗi sửa sản phẩm");
            }
        }

        public void Insert(ProductManage pm)
        {
            try
            {
                string query = "INSERT INTO SAN_PHAM VALUES('" + pm.MaSP + "', '" + pm.MaLoai + "', '" + pm.TenSP + "', '" + pm.MoTa + "', '" + pm.DonViTinh + "', '" + pm.GiaNiemYet + "', '" + pm.GiaBan + "', '" + pm.TrangThai + "')";
                MySqlCommand command = new MySqlCommand(query, connection);
                int r = command.ExecuteNonQuery();
                if (r > 0)
                {
                    Console.WriteLine("Thêm dữ liệu sản phẩm thành công");
                }
                else
                {
                    Console.WriteLine("Lỗi Thêm dữ liệu sản phẩm");
                }
            }
            catch
            {
                Console.WriteLine("Lỗi thêm sản phẩm");
            }
        }

        public bool CheckProduct(string ma)
        {
            string query = "SELECT COUNT(*) FROM SAN_PHAM WHERE MaSP = '"+ma+"' ";
            MySqlCommand command= new MySqlCommand(query, connection);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }
}