using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class InvoiceManagementService(IInvoiceManagementRepository repository)
{
    public IReadOnlyList<ManagedInvoice> GetInvoices() => repository.GetInvoices();
}
