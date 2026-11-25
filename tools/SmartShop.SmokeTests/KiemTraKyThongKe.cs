using SmartShop.BUS;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class KiemTraKyThongKe
{
    public static void Chay()
    {
        var dao = new KhoThongKeGiaLap();
        var bus = new DonHangBUS(dao);
        void DungKhoang(BoLocThoiGianThongKe boLoc, DateTime tuNgay, DateTime denTruocNgay, string moTa)
        {
            var ketQua = bus.ThongKeBanHang(boLoc);
            if (dao.Khoang != new KhoangThoiGianThongKe(tuNgay, denTruocNgay)
                || ketQua.From != tuNgay || ketQua.To != denTruocNgay)
                throw new Exception(moTa);
            Console.WriteLine("PASS PERIOD " + moTa);
        }
        void TuChoi(BoLocThoiGianThongKe boLoc, string moTa)
        {
            var soLanDoc = dao.SoLanDoc;
            try { bus.ThongKeBanHang(boLoc); }
            catch (ArgumentException)
            {
                if (dao.SoLanDoc != soLanDoc) throw new Exception("Bộ lọc lỗi vẫn gọi DAO: " + moTa);
                Console.WriteLine("PASS PERIOD " + moTa);
                return;
            }
            throw new Exception("Chưa từ chối: " + moTa);
        }

        DungKhoang(new() { TuNgay = new(2026, 11, 12, 15, 30, 0), DenNgay = new(2026, 11, 12, 18, 0, 0) },
            new(2026, 11, 12), new(2026, 11, 13), "Lấy đủ một ngày, bỏ giờ của đầu vào");
        DungKhoang(new() { TuNgay = new(2026, 12, 31), DenNgay = new(2027, 1, 1) },
            new(2026, 12, 31), new(2027, 1, 2), "Khoảng ngày qua năm mới bao gồm ngày cuối");
        DungKhoang(new() { Ky = KyThongKe.Thang, Nam = 2024, Thang = 2 },
            new(2024, 2, 1), new(2024, 3, 1), "Tháng hai năm nhuận gồm ngày 29");
        DungKhoang(new() { Ky = KyThongKe.Thang, Nam = 2026, Thang = 2 },
            new(2026, 2, 1), new(2026, 3, 1), "Tháng hai năm thường gồm 28 ngày");
        DungKhoang(new() { Ky = KyThongKe.Thang, Nam = 2026, Thang = 12 },
            new(2026, 12, 1), new(2027, 1, 1), "Tháng mười hai chuyển sang năm sau");
        for (var quy = 1; quy <= 4; quy++)
        {
            var tu = new DateTime(2026, (quy - 1) * 3 + 1, 1);
            DungKhoang(new() { Ky = KyThongKe.Quy, Nam = 2026, Quy = quy }, tu, tu.AddMonths(3), $"Quý {quy} đủ ba tháng");
        }
        DungKhoang(new() { Ky = KyThongKe.Nam, Nam = 2024 },
            new(2024, 1, 1), new(2025, 1, 1), "Cả năm nhuận gồm 366 ngày");
        DungKhoang(new() { Ky = KyThongKe.Nam, Nam = 9998 },
            new(9998, 1, 1), new(9999, 1, 1), "Năm lớn nhất hỗ trợ không tràn DateTime");
        TuChoi(new() { TuNgay = new(2026, 11, 13), DenNgay = new(2026, 11, 12) }, "Từ chối đảo khoảng ngày trước DAO");
        TuChoi(new() { TuNgay = default }, "Từ chối ngày rỗng trước DAO");
        TuChoi(new() { Ky = (KyThongKe)99 }, "Từ chối kỳ không hợp lệ trước DAO");
        TuChoi(new() { Ky = KyThongKe.Thang, Thang = 0 }, "Từ chối tháng 0 trước DAO");
        TuChoi(new() { Ky = KyThongKe.Thang, Thang = 13 }, "Từ chối tháng 13 trước DAO");
        TuChoi(new() { Ky = KyThongKe.Quy, Quy = 0 }, "Từ chối quý 0 trước DAO");
        TuChoi(new() { Ky = KyThongKe.Quy, Quy = 5 }, "Từ chối quý 5 trước DAO");
        TuChoi(new() { Ky = KyThongKe.Nam, Nam = 999 }, "Từ chối năm ngoài miền MySQL trước DAO");
        TuChoi(new() { Ky = KyThongKe.Nam, Nam = 9999 }, "Từ chối năm gây tràn kỳ tiếp theo trước DAO");
    }

    private sealed class KhoThongKeGiaLap : IDonHangDAO
    {
        public int SoLanDoc { get; private set; }
        public KhoangThoiGianThongKe? Khoang { get; private set; }
        public ThongKeBanHangDTO ThongKeBanHang(DateTime from, DateTime to)
        {
            SoLanDoc++;
            Khoang = new(from, to);
            return new(from, to, 0, 0, 0, 0);
        }
        public IReadOnlyList<DonHangQuanTriDTO> DocDonHang() => throw new NotSupportedException();
        public IReadOnlyList<KhoDTO> DocKho() => throw new NotSupportedException();
        public void CapNhatTrangThai(string orderCode, string status, string warehouseCode) => throw new NotSupportedException();
    }
}
