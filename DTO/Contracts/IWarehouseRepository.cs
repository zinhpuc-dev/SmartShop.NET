namespace SmartShop.DTO.Contracts;

public interface IWarehouseRepository
{
    IReadOnlyList<Warehouse> GetAll();
    Warehouse? GetByCode(string code);
    void Insert(Warehouse warehouse);
    void Update(Warehouse warehouse);
    void Delete(string code);
    IReadOnlyList<ReceivingRecord> GetReceivings();
    void CreateReceiving(ReceivingDraft draft);
    IReadOnlyList<ReceivingDetail> GetReceivingDetails(string code);
    IReadOnlyList<WarehouseStock> GetStock();
}
