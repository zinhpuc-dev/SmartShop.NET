namespace SmartShop.DTO.Contracts;

public interface IInvoiceManagementRepository
{
    IReadOnlyList<ManagedInvoice> GetInvoices();
    void CreateInvoice(string orderCode, string employeeCode, string paymentMethod);
    void MarkPaid(string invoiceCode);
}
