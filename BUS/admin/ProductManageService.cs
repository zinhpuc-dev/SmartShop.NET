using System;
using System.Collections.Generic;
using SmartShop.DTO;
using SmartShop.DAO;

namespace SmartShop.BUS
{
    public class ProductManageService
    {
        public List<ProductManage> product = new List<ProductManage>();


        public List<ProductManage> Read()
        {
            ProductManageDAO dao = new ProductManageDAO();
            if (product == null || product.Count == 0)
            {
                product = new List<ProductManage>();
                product = dao.ReadData();
            }

            return product;
        }

        public void UpDate(ProductManage pm)
        {
            Validate(pm);
            ProductManageDAO dao = new ProductManageDAO();
            dao.Update(pm);
            int index = product.FindIndex(x => x.MaSP == pm.MaSP);
            if (index != -1)
            {
                product[index] = pm;
            }
        }

        public void Insert(ProductManage pm)
        {
            Validate(pm);
            ProductManageDAO dao = new ProductManageDAO();
            if (dao.CheckProduct(pm.MaSP))
            {
                throw new Exception("Sản phẩm đã tồn tại");
            }
            dao.Insert(pm);
            product.Add(pm);
        }

        private static void Validate(ProductManage pm)
        {
            ArgumentNullException.ThrowIfNull(pm);
            if (string.IsNullOrWhiteSpace(pm.MaSP) || string.IsNullOrWhiteSpace(pm.MaLoai) ||
                string.IsNullOrWhiteSpace(pm.TenSP) || string.IsNullOrWhiteSpace(pm.DonViTinh) ||
                string.IsNullOrWhiteSpace(pm.TrangThai))
                throw new ArgumentException("Thông tin sản phẩm bắt buộc không được để trống.");
            if (pm.GiaNiemYet < 0 || pm.GiaBan < 0)
                throw new ArgumentException("Giá sản phẩm không được âm.");
        }
    }
}