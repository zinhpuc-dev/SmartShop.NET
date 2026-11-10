using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS
{
    public class ProductBUS
    {
        private readonly IProductManage dao;
        public List<ProductManage> product = new();

        public ProductBUS(IProductManage dao)
        {
            this.dao = dao ?? throw new ArgumentNullException(nameof(dao));
        }

        public List<ProductManage> Read()
        {
            product = dao.ReadData();
            return product;
        }

        public List<ProductManage> Search(string keyword)
        {
            return dao.Search(keyword);
        }

        public ProductManage? Get(string maSP) => dao.GetByCode(NormalizeCode(maSP, 8, "Mã sản phẩm"));

        public List<ProductCategory> ReadCategories() => dao.ReadCategories();

        public List<ProductStock> ReadStock(string maSP) => dao.ReadStockByProduct(NormalizeCode(maSP, 8, "Mã sản phẩm"));

        public void SaveCategory(ProductCategory category)
        {
            ArgumentNullException.ThrowIfNull(category);
            category.MaLoai = NormalizeCode(category.MaLoai, 6, "Mã loại sản phẩm");
            if (dao.CheckCategory(category.MaLoai)) dao.UpdateCategory(category);
            else dao.InsertCategory(category);
        }

        public void DeleteCategory(string maLoai)
        {
            dao.DeleteCategory(NormalizeCode(maLoai, 6, "Mã loại sản phẩm"));
        }

        public void UpDate(ProductManage pm)
        {
            Validate(pm);
            if (!dao.CheckProduct(pm.MaSP))
                throw new InvalidOperationException("Không tìm thấy sản phẩm.");
            if (!dao.CheckCategory(pm.MaLoai)) throw new ArgumentException("Không tìm thấy loại sản phẩm đã chọn.");
            dao.Update(pm);
            product = dao.ReadData();
        }

        public void Insert(ProductManage pm)
        {
            Validate(pm);
            if (dao.CheckProduct(pm.MaSP))
            {
                throw new Exception("Sản phẩm đã tồn tại");
            }
            if (!dao.ReadCategories().Any(c => c.MaLoai == pm.MaLoai && c.TrangThai))
                throw new ArgumentException("Vui lòng chọn loại sản phẩm đang hoạt động.");
            dao.Insert(pm);
            product = dao.ReadData();
        }

        public void Delete(string maSP)
        {
            string code = NormalizeCode(maSP, 8, "Mã sản phẩm");
            ProductManage? existing = dao.GetByCode(code);
            if (existing is null)
                throw new InvalidOperationException("Không tìm thấy sản phẩm.");
            existing.TrangThai = "0";
            dao.Update(existing);
            product = dao.ReadData();
        }

        private static void Validate(ProductManage pm)
        {
            ArgumentNullException.ThrowIfNull(pm);
            pm.MaSP = NormalizeCode(pm.MaSP, 8, "Mã sản phẩm");
            pm.MaLoai = NormalizeCode(pm.MaLoai, 6, "Mã loại sản phẩm");
            pm.TenSP = pm.TenSP.Trim();
            pm.DonViTinh = pm.DonViTinh.Trim();
            pm.MoTa = pm.MoTa?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(pm.MaSP) || string.IsNullOrWhiteSpace(pm.MaLoai) ||
                string.IsNullOrWhiteSpace(pm.TenSP) || string.IsNullOrWhiteSpace(pm.DonViTinh) ||
                string.IsNullOrWhiteSpace(pm.TrangThai))
                throw new ArgumentException("Thông tin sản phẩm bắt buộc không được để trống.");
            if (decimal.Round(pm.GiaNiemYet, 2) != pm.GiaNiemYet || decimal.Round(pm.GiaBan, 2) != pm.GiaBan)
                throw new ArgumentException("Giá sản phẩm chỉ được có tối đa hai chữ số thập phân.");
            if (pm.GiaNiemYet < 0 || pm.GiaBan < 0)
                throw new ArgumentException("Giá sản phẩm không được âm.");
            if (pm.TrangThai is not ("0" or "1"))
                throw new ArgumentException("Trạng thái sản phẩm không hợp lệ.");
            if (pm.TenSP.Length > 200 || pm.DonViTinh.Length > 30 || pm.MoTa.Length > 65535)
                throw new ArgumentException("Tên, đơn vị tính hoặc mô tả vượt quá độ dài cho phép.");
            if (pm.GiaNiemYet > 9999999999999999.99m || pm.GiaBan > 9999999999999999.99m)
                throw new ArgumentException("Giá sản phẩm vượt quá giới hạn lưu trữ.");
        }

        private static string NormalizeCode(string code, int maxLength, string label)
        {
            string normalized = code?.Trim().ToUpperInvariant() ?? string.Empty;
            if (normalized.Length is 0 || normalized.Length > maxLength)
                throw new ArgumentException($"{label} phải từ 1 đến {maxLength} ký tự.");
            return normalized;
        }
    }
}