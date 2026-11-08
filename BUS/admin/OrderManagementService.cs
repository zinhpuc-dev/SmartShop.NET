using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class OrderManagementService(IOrderManagementRepository repository)
{
    private static readonly HashSet<string> AllowedStatuses = ["Đang xử lý", "Đã xác nhận", "Đang giao hàng", "Đã giao", "Đã hủy", "Hoàn thành"];

    public IReadOnlyList<ManagedOrder> GetOrders() => repository.GetOrders();
    public IReadOnlyList<Warehouse> GetWarehouses() => repository.GetWarehouses();

    public void UpdateStatus(string orderCode, string status, string warehouseCode)
    {
        string code = Normalize(orderCode, 8, "Mã đơn hàng");
        string normalizedStatus = (status ?? string.Empty).Trim();
        if (!AllowedStatuses.Contains(normalizedStatus)) throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
        string warehouse = (warehouseCode ?? string.Empty).Trim().ToUpperInvariant();
        if (warehouse.Length > 6) throw new ArgumentException("Mã kho không hợp lệ.");
        if (normalizedStatus is "Đã giao" or "Hoàn thành" && warehouse.Length == 0)
            throw new ArgumentException("Cần chọn kho xuất hàng trước khi hoàn tất đơn.");
        repository.UpdateStatus(code, normalizedStatus, warehouse);
    }

    public SalesSummary GetSalesSummary(DateTime from, DateTime to)
    {
        if (from >= to) throw new ArgumentException("Khoảng thời gian báo cáo không hợp lệ.");
        return repository.GetSalesSummary(from, to);
    }

    private static string Normalize(string value, int maxLength, string label)
    {
        string code = (value ?? string.Empty).Trim().ToUpperInvariant();
        if (code.Length is 0 || code.Length > maxLength) throw new ArgumentException($"{label} phải từ 1 đến {maxLength} ký tự.");
        return code;
    }
}
