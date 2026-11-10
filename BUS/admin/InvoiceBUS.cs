using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class InvoiceBUS(IInvoiceManagementRepository repository)
{
    public IReadOnlyList<ManagedInvoice> GetInvoices() => repository.GetInvoices();

    public void CreateInvoice(string orderCode, string employeeCode, string paymentMethod)
    {
        string order = (orderCode ?? "").Trim().ToUpperInvariant();
        string employee = (employeeCode ?? "").Trim().ToUpperInvariant();
        if (order.Length is 0 or > 8 || employee.Length is 0 or > 8)
            throw new ArgumentException("Vui lòng chọn đơn hàng và nhân viên lập hóa đơn.");
        if (paymentMethod is not ("Tiền mặt" or "Chuyển khoản" or "COD"))
            throw new ArgumentException("Phương thức thanh toán không hợp lệ.");
        repository.CreateInvoice(order, employee, paymentMethod);
    }

    public void MarkPaid(string invoiceCode)
    {
        string code = (invoiceCode ?? "").Trim().ToUpperInvariant();
        if (code.Length is 0 or > 8) throw new ArgumentException("Mã hóa đơn không hợp lệ.");
        repository.MarkPaid(code);
    }
}
