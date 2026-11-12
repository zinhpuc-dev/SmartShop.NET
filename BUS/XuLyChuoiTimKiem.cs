using System.Globalization;
using System.Text;

namespace SmartShop.BUS;

internal static class XuLyChuoiTimKiem
{
    public static string ChuanHoa(string value)
    {
        var decomposed = value.Normalize(NormalizationForm.FormD);
        return new string(decomposed.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray()).Replace('đ', 'd').Replace('Đ', 'D');
    }
}
