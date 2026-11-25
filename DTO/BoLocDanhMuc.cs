namespace SmartShop.DTO;

public enum PhepSoSanh
{
    Chua, Bang, Khac, LonHon, LonHonHoacBang, NhoHon, NhoHonHoacBang
}

public enum KieuTruongTimKiem { VanBan, So }

public sealed record TruongTimKiem(string Key, string Label, KieuTruongTimKiem Type = KieuTruongTimKiem.VanBan);
public sealed record DieuKienTimKiem(string Field, PhepSoSanh Operator, string Value, bool Negate = false);

// Các điều kiện trong cùng nhóm nối bằng AND; các nhóm nối với nhau bằng OR.
public sealed record NhomDieuKien(IReadOnlyList<DieuKienTimKiem> Conditions);
public sealed record BoLocDanhMuc(IReadOnlyList<NhomDieuKien> Groups);
