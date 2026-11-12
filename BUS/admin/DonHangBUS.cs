using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class DonHangBUS(IDonHangDAO repository)
{
    private static readonly HashSet<string> AllowedStatuses = ["Đang xử lý", "Đã xác nhận", "Đang giao hàng", "Đã giao", "Đã hủy", "Hoàn thành"];

    public IReadOnlyList<DonHangQuanTriDTO> DocDonHang() => repository.DocDonHang();
    public IReadOnlyList<KhoDTO> DocKho() => repository.DocKho();

    public void CapNhatTrangThai(string orderCode, string status, string warehouseCode)
    {
        string code = ChuanHoa(orderCode, 8, "Mã đơn hàng");
        string normalizedStatus = (status ?? string.Empty).Trim();
        if (!AllowedStatuses.Contains(normalizedStatus)) throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
        string warehouse = (warehouseCode ?? string.Empty).Trim().ToUpperInvariant();
        if (warehouse.Length > 6) throw new ArgumentException("Mã kho không hợp lệ.");
        if (normalizedStatus is "Đã giao" or "Hoàn thành" && warehouse.Length == 0)
            throw new ArgumentException("Cần chọn kho xuất hàng trước khi hoàn tất đơn.");
        repository.CapNhatTrangThai(code, normalizedStatus, warehouse);
    }

    public ThongKeBanHangDTO ThongKeBanHang(DateTime from, DateTime to)
    {
        if (from >= to) throw new ArgumentException("Khoảng thời gian báo cáo không hợp lệ.");
        return repository.ThongKeBanHang(from, to);
    }

    private static string ChuanHoa(string value, int maxLength, string label)
    {
        string code = (value ?? string.Empty).Trim().ToUpperInvariant();
        if (code.Length is 0 || code.Length > maxLength) throw new ArgumentException($"{label} phải từ 1 đến {maxLength} ký tự.");
        return code;
    }
}
