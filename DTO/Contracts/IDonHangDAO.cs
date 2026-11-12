namespace SmartShop.DTO.Contracts;

public interface IDonHangDAO
{
    IReadOnlyList<DonHangQuanTriDTO> DocDonHang();
    void CapNhatTrangThai(string orderCode, string status, string warehouseCode);
    IReadOnlyList<KhoDTO> DocKho();
    ThongKeBanHangDTO ThongKeBanHang(DateTime from, DateTime to);
}
