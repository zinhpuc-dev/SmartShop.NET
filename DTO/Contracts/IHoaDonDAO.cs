namespace SmartShop.DTO.Contracts;

public interface IHoaDonDAO
{
    IReadOnlyList<HoaDonDTO> DocHoaDon();
    void LapHoaDon(string orderCode, string employeeCode, string paymentMethod);
    void DanhDauDaThanhToan(string invoiceCode);
}
