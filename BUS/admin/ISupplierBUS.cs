using SmartShop.DTO;

namespace SmartShop.BUS;

public interface ISupplierBUS
{
    void Reload();
    IReadOnlyList<Supplier> Search(string query = "");
    Supplier Get(string code);
    string SuggestCode();
    Supplier Create(SupplierDraft draft);
    Supplier Update(string code, SupplierDraft draft);
    void Delete(string code);
}
