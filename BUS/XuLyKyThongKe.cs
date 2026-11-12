using SmartShop.DTO;

namespace SmartShop.BUS;

public static class XuLyKyThongKe
{
    public static KhoangThoiGianThongKe ChuyenThanhKhoang(BoLocThoiGianThongKe boLoc)
    {
        ArgumentNullException.ThrowIfNull(boLoc);
        if (!Enum.IsDefined(boLoc.Ky)) throw new ArgumentException("Kỳ thống kê không hợp lệ.");

        if (boLoc.Ky == KyThongKe.KhoangNgay)
        {
            KiemTraNam(boLoc.TuNgay.Year);
            KiemTraNam(boLoc.DenNgay.Year);
            if (boLoc.TuNgay.Date > boLoc.DenNgay.Date)
                throw new ArgumentException("Ngày bắt đầu không được sau ngày kết thúc.");
            return new(boLoc.TuNgay.Date, boLoc.DenNgay.Date.AddDays(1));
        }

        KiemTraNam(boLoc.Nam);
        if (boLoc.Ky == KyThongKe.Thang && boLoc.Thang is < 1 or > 12)
            throw new ArgumentException("Tháng phải từ 1 đến 12.");
        if (boLoc.Ky == KyThongKe.Quy && boLoc.Quy is < 1 or > 4)
            throw new ArgumentException("Quý phải từ 1 đến 4.");

        var tuNgay = new DateTime(boLoc.Nam, boLoc.Ky switch
        {
            KyThongKe.Thang => boLoc.Thang,
            KyThongKe.Quy => (boLoc.Quy - 1) * 3 + 1,
            _ => 1
        }, 1);
        return new(tuNgay, tuNgay.AddMonths(boLoc.Ky switch
        {
            KyThongKe.Thang => 1,
            KyThongKe.Quy => 3,
            _ => 12
        }));
    }

    private static void KiemTraNam(int nam)
    {
        // Chừa năm kế tiếp để biểu diễn cận trên loại trừ của kỳ cuối năm.
        if (nam is < 1000 or > 9998) throw new ArgumentException("Năm thống kê phải từ 1000 đến 9998.");
    }
}
