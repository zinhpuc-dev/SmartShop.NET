using System.Globalization;

namespace SmartShop.UI.Shared;

public static class DinhDangTienTe
{
    private static readonly CultureInfo Vietnamese = CultureInfo.GetCultureInfo("vi-VN");
    public static string DinhDang(decimal value) => value.ToString("N0", Vietnamese) + " ₫";
}
