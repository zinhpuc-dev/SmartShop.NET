namespace SmartShop.DTO.Contracts;

public interface IInvoiceManagementRepository
{
    IReadOnlyList<ManagedInvoice> GetInvoices();
}
