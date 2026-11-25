namespace SmartShop.DTO;

public enum KyThongKe { KhoangNgay, Thang, Quy, Nam }

public sealed record BoLocThoiGianThongKe
{
    public KyThongKe Ky { get; init; } = KyThongKe.KhoangNgay;
    public DateTime TuNgay { get; init; } = DateTime.Today.AddDays(-29);
    public DateTime DenNgay { get; init; } = DateTime.Today;
    public int Nam { get; init; } = DateTime.Today.Year;
    public int Thang { get; init; } = DateTime.Today.Month;
    public int Quy { get; init; } = (DateTime.Today.Month - 1) / 3 + 1;
}

// Bao gồm TuNgay, không bao gồm DenTruocNgay để lấy đủ giao dịch ngày cuối kỳ.
public sealed record KhoangThoiGianThongKe(DateTime TuNgay, DateTime DenTruocNgay);
