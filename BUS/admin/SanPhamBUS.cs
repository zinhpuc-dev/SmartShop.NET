using System.Collections;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS
{
    public class SanPhamBUS
    {
        private readonly ISanPhamDAO dao;
        public List<SanPhamQuanTriDTO> product = new();
        private ArrayList? products;

        public SanPhamBUS(ISanPhamDAO dao)
        {
            this.dao = dao ?? throw new ArgumentNullException(nameof(dao));
        }

        public List<SanPhamQuanTriDTO> DocDanhSach()
        {
            products = null;
            var loaded = dao.DocDanhSach();
            products = new ArrayList(loaded.Select(value => value with { }).ToArray());
            product = loaded;
            return loaded;
        }

        public List<SanPhamQuanTriDTO> TimKiem(string keyword)
        {
            return TimKiem(keyword, null);
        }

        public List<SanPhamQuanTriDTO> TimKiem(string? keyword, string? category, BoLocDanhMuc? filter = null)
        {
            var matches = TimKiemDanhMucNangCao.LocSanPham(filter);
            if (products is null) DocDanhSach();
            var term = XuLyChuoiTimKiem.ChuanHoa((keyword ?? "").Trim());
            return products!.Cast<SanPhamQuanTriDTO>()
                .Where(matches)
                .Where(value => (string.IsNullOrEmpty(category) || value.MaLoai == category)
                    && (term.Length == 0
                        || XuLyChuoiTimKiem.ChuanHoa(value.MaSP).Contains(term, StringComparison.OrdinalIgnoreCase)
                        || XuLyChuoiTimKiem.ChuanHoa(value.TenSP).Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Select(value => value with { }).ToList();
        }

        public SanPhamQuanTriDTO? LayTheoMa(string maSP) => dao.DocTheoMa(ChuanHoaMa(maSP, 8, "Mã sản phẩm"));

        public List<LoaiSanPhamDTO> DocLoaiSanPham() => dao.DocLoaiSanPham();

        public List<TonKhoSanPhamDTO> DocTonKho(string maSP) => dao.DocTonKhoTheoSanPham(ChuanHoaMa(maSP, 8, "Mã sản phẩm"));

        public void LuuLoaiSanPham(LoaiSanPhamDTO category)
        {
            ArgumentNullException.ThrowIfNull(category);
            category.MaLoai = ChuanHoaMa(category.MaLoai, 6, "Mã loại sản phẩm");
            if (dao.KiemTraLoaiSanPham(category.MaLoai)) dao.SuaLoaiSanPham(category);
            else dao.ThemLoaiSanPham(category);
        }

        public void XoaLoaiSanPham(string maLoai)
        {
            dao.XoaLoaiSanPham(ChuanHoaMa(maLoai, 6, "Mã loại sản phẩm"));
        }

        public void Sua(SanPhamQuanTriDTO pm)
        {
            KiemTraDuLieu(pm);
            if (!dao.KiemTraSanPham(pm.MaSP))
                throw new InvalidOperationException("Không tìm thấy sản phẩm.");
            if (!dao.KiemTraLoaiSanPham(pm.MaLoai)) throw new ArgumentException("Không tìm thấy loại sản phẩm đã chọn.");
            dao.Sua(pm);
            DocDanhSach();
        }

        public void Them(SanPhamQuanTriDTO pm)
        {
            KiemTraDuLieu(pm);
            if (dao.KiemTraSanPham(pm.MaSP))
            {
                throw new Exception("Sản phẩm đã tồn tại");
            }
            if (!dao.DocLoaiSanPham().Any(c => c.MaLoai == pm.MaLoai && c.TrangThai))
                throw new ArgumentException("Vui lòng chọn loại sản phẩm đang hoạt động.");
            dao.Them(pm);
            DocDanhSach();
        }

        public void Xoa(string maSP)
        {
            string code = ChuanHoaMa(maSP, 8, "Mã sản phẩm");
            dao.Xoa(code);
            DocDanhSach();
        }

        public void DoiTrangThai(string maSP, bool active)
        {
            string code = ChuanHoaMa(maSP, 8, "Mã sản phẩm");
            if (!dao.KiemTraSanPham(code)) throw new InvalidOperationException("Không tìm thấy sản phẩm.");
            dao.DoiTrangThai(code, active);
            DocDanhSach();
        }

        private static void KiemTraDuLieu(SanPhamQuanTriDTO pm)
        {
            ArgumentNullException.ThrowIfNull(pm);
            pm.MaSP = ChuanHoaMa(pm.MaSP, 8, "Mã sản phẩm");
            pm.MaLoai = ChuanHoaMa(pm.MaLoai, 6, "Mã loại sản phẩm");
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

        private static string ChuanHoaMa(string code, int maxLength, string label)
        {
            string normalized = code?.Trim().ToUpperInvariant() ?? string.Empty;
            if (normalized.Length is 0 || normalized.Length > maxLength)
                throw new ArgumentException($"{label} phải từ 1 đến {maxLength} ký tự.");
            return normalized;
        }
    }
}