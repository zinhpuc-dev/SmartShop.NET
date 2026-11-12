using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class HoaDonBUS(IHoaDonDAO repository)
{
    public IReadOnlyList<HoaDonDTO> DocHoaDon() => repository.DocHoaDon();

    public void LapHoaDon(string orderCode, string employeeCode, string paymentMethod)
    {
        string order = (orderCode ?? "").Trim().ToUpperInvariant();
        string employee = (employeeCode ?? "").Trim().ToUpperInvariant();
        if (order.Length is 0 or > 8 || employee.Length is 0 or > 8)
            throw new ArgumentException("Vui lòng chọn đơn hàng và nhân viên lập hóa đơn.");
        if (paymentMethod is not ("Tiền mặt" or "Chuyển khoản" or "COD"))
            throw new ArgumentException("Phương thức thanh toán không hợp lệ.");
        repository.LapHoaDon(order, employee, paymentMethod);
    }

    public void DanhDauDaThanhToan(string invoiceCode)
    {
        string code = (invoiceCode ?? "").Trim().ToUpperInvariant();
        if (code.Length is 0 or > 8) throw new ArgumentException("Mã hóa đơn không hợp lệ.");
        repository.DanhDauDaThanhToan(code);
    }
}
