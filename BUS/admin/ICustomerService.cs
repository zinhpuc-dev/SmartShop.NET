using SmartShop.DTO;

namespace SmartShop.BUS;

public interface ICustomerService
{
    IReadOnlyList<Customer> Search(string query = "");
    Customer Get(string code);
    string SuggestCode();
    Customer Create(CustomerDraft draft);
    Customer Update(string code, CustomerDraft draft);
    void Delete(string code);
}
